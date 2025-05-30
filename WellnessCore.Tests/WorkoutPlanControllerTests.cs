using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Controllers;
using API.Data;
using API.Entities;
using API.Dtos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;

// DTO for grouping exercises by body part

public class WorkoutPlanControllerTests
{
    private readonly DbContextOptions<DataContext> _dbOptions;

    public WorkoutPlanControllerTests()
    {
        _dbOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: $"WorkoutPlanTestDb_{System.Guid.NewGuid()}")
            .Options;
    }

    [Fact]
    public async Task GetUserWorkoutPlan_ReturnsGroupedExercises_WhenValidInput()
    {
        // Arrange
        await using var context = new DataContext(_dbOptions);

        var exercises = new List<Exercise>
        {
            new Exercise { Id = 1, Name = "Push Up", BodyPart = "chest", Equipment = "Body Weight", Target = "Chest", GifUrl = "url1" },
            new Exercise { Id = 2, Name = "Plank", BodyPart = "waist", Equipment = "Body Weight", Target = "Core", GifUrl = "url2" },
            new Exercise { Id = 3, Name = "Bicep Curl", BodyPart = "upper arms", Equipment = "Dumbbell", Target = "Biceps", GifUrl = "url3" }
        };

        var plan = new WorkoutPlan
        {
            Id = 1,
            Title = "Beginner Muscle Plan",
            Goal = "Build Muscle",
            FitnessLevel = "Beginner",
            Type = "predefined",
            WorkoutPlanExercises = new List<WorkoutPlanExercise>
            {
                new WorkoutPlanExercise { WorkoutPlanId = 1, ExerciseId = 1, Day = 1, Order = 1, Exercise = exercises[0] },
                new WorkoutPlanExercise { WorkoutPlanId = 1, ExerciseId = 2, Day = 2, Order = 1, Exercise = exercises[1] },
                new WorkoutPlanExercise { WorkoutPlanId = 1, ExerciseId = 3, Day = 3, Order = 1, Exercise = exercises[2] }
            }
        };

        context.Exercises.AddRange(exercises);
        context.WorkoutPlans.Add(plan);
        await context.SaveChangesAsync();

        var userRepoMock = new Mock<API.Interfaces.IUserRepository>();
        var controller = new UsersController(userRepoMock.Object, context);

        // Act
        var result = await controller.GetUserWorkoutPlan("Beginner", "Build Muscle");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var bodyPartGroups = Assert.IsType<List<BodyPartExercisesDto>>(okResult.Value);

        Assert.Contains(bodyPartGroups, g => g.BodyPart == "chest");
        Assert.Contains(bodyPartGroups, g => g.BodyPart == "core");
        Assert.Contains(bodyPartGroups, g => g.BodyPart == "arms");
    }
}
