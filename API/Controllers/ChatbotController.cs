using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Dtos;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace API.Controllers;

[Authorize]
public class ChatbotController(DataContext context, IHttpClientFactory httpClientFactory) : BaseApiController
{
    [HttpPost("chat")]
    public async Task<ActionResult<ChatResponseDto>> ChatWithCoach([FromBody] ChatMessageDto userMessage)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var user = await context.Users.FindAsync(userId);
        if (user == null) return Unauthorized();

        var userGoal = string.IsNullOrWhiteSpace(user.Traineegoal) ? "general fitness" : user.Traineegoal;

        var messages = new List<object>
        {
            new {
                role = "system",
                content = $"""
                You are a certified fitness and nutrition coach.
                The user is a {user.Gender}, {user.Weight}kg, {user.Height}cm tall, and their goal is {userGoal}.
                If they request a workout plan, ask how many days they can train per week. If they tell you, tailor the plan to that number. Otherwise, offer a flexible plan for 3 to 6 days, with instructions on how to adapt it based on time availability.
                Provide clear sets, reps, and rest suggestions. Always be helpful.
                """
            },
            new {
                role = "user",
                content = userMessage.Content
            }
        };

        var client = httpClientFactory.CreateClient();
        var apiKey = Environment.GetEnvironmentVariable("OpenAIApiKey");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var body = new
        {
            model = "gpt-4o",
            messages = messages,
            temperature = 0.7
        };

        var content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("https://api.openai.com/v1/chat/completions", content);
        var raw = await response.Content.ReadAsStringAsync();

        Console.WriteLine("OpenAI raw response:\n" + raw);

        string? reply = null;
        try
        {
            dynamic json = JsonConvert.DeserializeObject(raw)!;
            reply = json?.choices?[0]?.message?.content?.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to extract reply: " + ex.Message);
        }

        if (string.IsNullOrWhiteSpace(reply))
        {
            reply = "I'm currently unable to process that. Please try rephrasing or ask something else about your training or nutrition.";
        }

        context.ChatMessages.Add(new ChatMessage { UserId = userId, Role = "user", Content = userMessage.Content });
        context.ChatMessages.Add(new ChatMessage { UserId = userId, Role = "assistant", Content = reply });
        await context.SaveChangesAsync();

        return Ok(new ChatResponseDto
        {
            Messages = new List<ChatMessageDto>
            {
                new() { Role = "assistant", Content = reply }
            }
        });
    }

    [HttpGet("history")]
    public async Task<ActionResult<List<ChatMessageDto>>> GetChatHistory()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var history = await context.ChatMessages
            .Where(x => x.UserId == userId)
            .OrderBy(x => x.CreatedAt)
            .Select(x => new ChatMessageDto
            {
                Role = x.Role,
                Content = x.Content
            })
            .ToListAsync();

        return Ok(history);
    }
}
