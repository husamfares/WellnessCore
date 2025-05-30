using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Dtos;
// Add the following using if BodyPartExercisesDto is in a different namespace
// using API.DTOs; // Ensure this is correct and BodyPartExercisesDto exists in API.DTOs

public class HomeWorkoutControllerTests
{
    private readonly DbContextOptions<DataContext> _dbOptions;

    public HomeWorkoutControllerTests()
    {
        _dbOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: $"HomeWorkoutTestDb_{System.Guid.NewGuid()}")
            .Options;
    }
[Fact]
public async Task GetHomeWorkout_ReturnsGroupedExercises_WhenValidInput()
{
    // Arrange
    using var context = new DataContext(_dbOptions);

    // Seed exercises
    var exercises = new List<Exercise>
    {
        new Exercise { Id = 1, Name = "Push-up", Equipment = "body weight", BodyPart = "chest", Target = "chest" },
        new Exercise { Id = 2, Name = "Squat", Equipment = "body weight", BodyPart = "upper legs", Target = "quads" },
        new Exercise { Id = 3, Name = "Plank", Equipment = "body weight", BodyPart = "waist", Target = "abs" },
        new Exercise { Id = 4, Name = "Lunge", Equipment = "body weight", BodyPart = "lower legs", Target = "calves" }
    };
    await context.Exercises.AddRangeAsync(exercises);
    await context.SaveChangesAsync();

    // Seed workout plan
    var workoutPlan = new WorkoutPlan
    {
        Id = 1,
        Goal = "Build Muscle",
        FitnessLevel = "Beginner",
        Type = "home", // ensure this matches controller logic if needed
        WorkoutPlanExercises = exercises.Select((e, i) => new WorkoutPlanExercise
        {
            WorkoutPlanId = 1,
            ExerciseId = e.Id,
            Day = 1,
            Order = i + 1
        }).ToList()
    };
    context.WorkoutPlans.Add(workoutPlan);
    await context.SaveChangesAsync();

    // Set up controller
    var userRepo = new Moq.Mock<API.Interfaces.IUserRepository>().Object;
    var controller = new UsersController(userRepo, context);

    // Act
   var result = await controller.getHomeWorkout("Beginner", "Build Muscle");

Assert.NotNull(result);

if (result is NotFoundObjectResult nf)
{
    throw new Exception($"Unexpected NotFound: {nf.Value}");
}

var okResult = result as OkObjectResult;
Assert.NotNull(okResult);

if (okResult.Value == null)
{
    throw new Exception("OkObjectResult returned null value.");
}

Console.WriteLine($"Returned type: {okResult.Value.GetType().FullName}");

// SAFELY cast without crashing
var data = okResult.Value as List<BodyPartExercisesDto>;
Assert.NotNull(data);
Assert.NotEmpty(data);

}

}
