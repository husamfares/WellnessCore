using System;
using System.Net.Http.Headers;
using System.Text;
using API.Data;
using API.Dtos;
using API.Entities;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace API.Controllers;

[Authorize]
public class MealAnalyzerController(DataContext context, CloudinaryService cloudinaryService, IConfiguration configuration, IHttpClientFactory httpClientFactory) : BaseApiController
{
    [HttpPost("analyze-image")]
    public async Task<IActionResult> AnalyzeImage([FromForm] IFormFile image)
    {
        if (image == null || image.Length == 0)
            return BadRequest("No image uploaded");

        // Upload to Cloudinary
        var imageUrl = await cloudinaryService.UploadImageAsync(image);

        // Convert to base64
        using var ms = new MemoryStream();
        await image.CopyToAsync(ms);
        var base64Image = Convert.ToBase64String(ms.ToArray());

        // Call OpenAI GPT-4o
        var client = httpClientFactory.CreateClient();
        var apiKey = Environment.GetEnvironmentVariable("OpenAIApiKey");

        if (string.IsNullOrEmpty(apiKey))
            return StatusCode(500, "OpenAI API key not found in environment variables");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var body = new
        {
            model = "gpt-4o",
            temperature = 0,
            messages = new object[]
            {
                new {
                    role = "user",
                    content = new object[]
                    {
                        new { type = "text", text = "Estimate the food name, calories and protein in this image. Respond ONLY with JSON like this: { \"food\": \"string\", \"calories\": number, \"protein_g\": number } with no extra text." },
                        new { type = "image_url", image_url = new { url = $"data:image/jpeg;base64,{base64Image}" } }
                    }
                }
            }
        };

        var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
        var raw = await response.Content.ReadAsStringAsync();

        try
        {
            dynamic parsed = JsonConvert.DeserializeObject(raw)!;
            string? messageContent = parsed?.choices?[0]?.message?.content?.ToString();

            if (string.IsNullOrWhiteSpace(messageContent))
                return StatusCode(500, $"OpenAI response was empty. Raw: {raw}");

            var start = messageContent.IndexOf('{');
            var end = messageContent.LastIndexOf('}');
            if (start == -1 || end == -1)
                return BadRequest($"Invalid format in OpenAI message. Raw content: {messageContent}");

            var json = messageContent.Substring(start, end - start + 1);
            Console.WriteLine("Parsed JSON from OpenAI:\n" + json);

            var rawObj = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

            var result = new MealAnalysis
            {
                Food = rawObj?["food"]?.ToString() ?? "Unknown",
                Calories = int.TryParse(rawObj?["calories"]?.ToString(), out var cal) ? cal : 0,
                Protein = double.TryParse(rawObj?["protein_g"]?.ToString(), out var pro) ? pro : 0,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow,
            };

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Invalid user id");

            result.UserId = userId;

            context.MealAnalyses.Add(result);
            await context.SaveChangesAsync();

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Failed to parse OpenAI response: {ex.Message}");
        }
    }

    [HttpGet("history")]
    public async Task<ActionResult<IEnumerable<MealAnalysisDto>>> GetHistory()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out int userId))
            return Unauthorized("Invalid user id");

        var history = await context.MealAnalyses
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new MealAnalysisDto
            {
                ImageUrl = x.ImageUrl,
                Food = x.Food,
                Calories = x.Calories,
                ProteinG = x.Protein,
                CreatedAt = x.CreatedAt
            })
            .ToListAsync();

        return Ok(history);
    }
}
