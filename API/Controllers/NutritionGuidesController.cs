using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Dtos;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

    [ApiController]
    [Route("api/[controller]")]
    public class NutritionGuidesController : ControllerBase
    {
        private readonly DataContext _context;

        public NutritionGuidesController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("get-user-guide")]
        public async Task<IActionResult> GetUserNutritionGuide([FromBody] NutritionGuideRequestDto request)
        {
            var user = await _context.Users
                .Where(u => u.Id == request.UserId)
                .Select(u => new {
                    u.DateOfBirth,
                    u.Gender,
                    u.Traineegoal
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return NotFound("User not found");


                var today = DateTime.Today;
                int age = today.Year - user.DateOfBirth.Year;
               if (user.DateOfBirth > DateOnly.FromDateTime(today.AddYears(-age))) age--;


            var guide = await _context.NutritionGuides
                .Where(g =>
                    age >= g.AgeRangeStart &&
                    age <= g.AgeRangeEnd &&
                    g.Gender.ToLower() == (user.Gender ?? "").ToLower() &&
                    g.Goal.ToLower() == (user.Traineegoal ?? "").ToLower())
                    .FirstOrDefaultAsync();

            if (guide == null)
                return NotFound("No suitable nutrition guide found");

            return Ok(guide);
        }
    }

