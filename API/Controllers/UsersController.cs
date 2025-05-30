using System;
using System.Text.RegularExpressions;
using API.Data;
using API.Dtos;
using API.Entities;
using API.Interfaces;
using API.Migrations;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserRepository userRepository, DataContext context) : BaseApiController
{

    [AllowAnonymous]
    [HttpGet] // api/users     (( will give you all users ))
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await userRepository.GetMembersAsync();


        return Ok(users);
    }


    [HttpGet("{username}")] // api/users/ Alaa 
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var user = await userRepository.GetMemberAsync(username);

        if (user == null) return NotFound();

        return user;


    }

    [HttpGet("user/fitness-check")]
    public async Task<ActionResult<MemberDto>> GetFitnessCheckInfo()
    {
        var username = User.Identity?.Name;

        if (string.IsNullOrEmpty(username)) return Unauthorized("User not logged in");

        var user = await userRepository.GetMemberAsync(username);

        if (user == null) return NotFound("User not found");

        return Ok(user);


    }

    [HttpPut("user/update-fitness-value")]

    public async Task<ActionResult> UpdateFitnessLevel(FitnessCheckUpdate fitnessCheckUpdate)
    {

        if (fitnessCheckUpdate.FitnessLevel == null || fitnessCheckUpdate.Traineegoal == null)
        {
            return BadRequest("no fitnessLevel or no goal");
        }
        var user = User.Identity?.Name;


        if (string.IsNullOrEmpty(user)) return Unauthorized("User not logged in");

        var username = await userRepository.GetUserByUsernameAsync(user);

        if (username == null) return BadRequest("there is no username inside DB");

        username.FitnessLevel = fitnessCheckUpdate.FitnessLevel;
        username.Traineegoal = fitnessCheckUpdate.Traineegoal;

        userRepository.Update(username); // optional if you're using tracking
        var result = await context.SaveChangesAsync();

        if (result > 0)
            return Ok(new { message = "Fitness level updated successfully" });
        else
            return BadRequest("something happen when update");


        // return BadRequest("Failed to update fitness level");
    }

    [HttpGet("workout/standard-workout")]

    public async Task<ActionResult> GetUserWorkoutPlan([FromQuery] string traineeLevel, [FromQuery] string traineeGoal)
    {
        if (traineeLevel == null || traineeGoal == null)
            return BadRequest("trainee level and goal is null");

        var plan = await context.WorkoutPlans
        .Where(p => p.Goal == traineeGoal && p.FitnessLevel == traineeLevel && p.Type == "predefined")
        .Include(p => p.WorkoutPlanExercises!)
        .ThenInclude(wpe => wpe.Exercise)
        .FirstOrDefaultAsync();

        if (plan == null || plan.WorkoutPlanExercises == null)
        {
            return NotFound("No workout plan found for the given level and goal.");
        }

        var result = plan.WorkoutPlanExercises
                .Select(wpe => wpe.Exercise!)
                .GroupBy(e => MapBodyPartToClassification(e.BodyPart))
                .Select(group => new BodyPartExercisesDto
                {
                    BodyPart = group.Key,
                    Exercises = group.Select(e => new ExerciseDto
                    {
                        Id = e.Id.ToString(),
                        Name = e.Name,
                        Equipment = e.Equipment,
                        Target = e.Target,
                        GifUrl = e.GifUrl
                    }).ToList()
                })
        .ToList();

        return Ok(result);
    }


    [AllowAnonymous]
    [HttpGet("workout/exercise-gif")]
    public async Task<IActionResult> GetExerciseGif([FromQuery] int exerciseId)
    {
        var exercise = await context.Exercises.FirstOrDefaultAsync(e => e.Id == exerciseId);

        if (exercise == null)
        {
            return NotFound("Exercise not found.");
        }

        // Always get latest gif from ExerciseDB:
        if (string.IsNullOrWhiteSpace(exercise.Name))
        {
            return BadRequest("Exercise name is invalid.");
        }

        var gifUrl = await GetLatestGifUrlFromExerciseDbAsync(exercise.Name);

        if (string.IsNullOrWhiteSpace(gifUrl))
        {
            return NotFound("GIF not found in ExerciseDB.");
        }

        return Redirect(gifUrl);
    }

    [HttpGet("workout/home-workout")]
    public async Task<ActionResult> getHomeWorkout([FromQuery] string fitnessLevel, [FromQuery] string traineeGoal)
    {
        if (fitnessLevel == null || traineeGoal == null)
            return BadRequest("trainee level and goal is null");

        var homePlan = await context.WorkoutPlans
                        .Where(e => e.FitnessLevel == fitnessLevel && e.Goal == traineeGoal)
                        .Include(ex => ex.WorkoutPlanExercises!)
                        .ThenInclude(exp => exp.Exercise)
                        .FirstOrDefaultAsync();


        if (homePlan == null || homePlan.WorkoutPlanExercises == null)
        {
            return NotFound("No workout homePlan found for the given level and goal.");
        }

        var result = homePlan.WorkoutPlanExercises
               .Select(wpe => wpe.Exercise!)
               .Where(e => e.Equipment != null && e.Equipment.ToLower() == "body weight")
               .GroupBy(e => MapBodyPartToClassification(e.BodyPart))
               .Select(group => new BodyPartExercisesDto
               {
                   BodyPart = group.Key,
                   Exercises = group.Take(3).Select(e => new ExerciseDto
                   {
                       Id = e.Id.ToString(),
                       Name = e.Name,
                       Equipment = e.Equipment,
                       Target = e.Target,
                       GifUrl = e.GifUrl,
                       Instructions = e.Instructions
                   }).ToList()
               })
               .ToList();

        return Ok(result);

    }




    private string MapBodyPartToClassification(string? bodyPart)
    {
        if (string.IsNullOrWhiteSpace(bodyPart))
            return "other";

        bodyPart = bodyPart.ToLower();

        return bodyPart switch
        {
            "chest" => "chest",
            "back" => "back",
            "waist" => "core",
            "lower arms" or "upper arms" => "arms",
            "lower legs" or "upper legs" => "legs",
            "shoulders" => "shoulders",
            "cardio" => "cardio",
            _ => "other"
        };
    }

    private readonly string RapidApiKey = "f13ec23faamsh77fa19b43fd4ac8p172375jsn16d31dbcf104"; // your RapidAPI key

    private async Task<string?> GetLatestGifUrlFromExerciseDbAsync(string exerciseName)
    {
        using var httpClient = new HttpClient();

        var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"https://exercisedb.p.rapidapi.com/exercises/name/{Uri.EscapeDataString(exerciseName)}"
        );

        request.Headers.Add("X-RapidAPI-Key", RapidApiKey);
        request.Headers.Add("X-RapidAPI-Host", "exercisedb.p.rapidapi.com");

        var response = await httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return null;

        var exercises = await response.Content.ReadFromJsonAsync<List<ExerciseDto>>();

        return exercises?.FirstOrDefault()?.GifUrl;
    }

    [HttpGet("workout/therapist-exercises")]
    public async Task<ActionResult> getThrapistExercises()
    {
        var exercises = await context.TherapistExercises.ToListAsync();

        var result = exercises.Select(e => new TherapistExercisesDto
        {
            Name = e.Name,
            TargetArea = e.TargetArea,
            SuitableFor = e.SuitableFor,
            Instructions = e.Instructions,
            YoutubeUrl = e.YoutubeUrl
        }).ToList();

        return Ok(result);     

    }


}
