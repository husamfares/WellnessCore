using Xunit;
using Microsoft.EntityFrameworkCore;
using API.Controllers;
using API.Data;
using API.Entities;
using API.Dtos;
using Microsoft.AspNetCore.Mvc;


public class TherapistExercisesControllerTests
{
    private readonly DbContextOptions<DataContext> _dbOptions;

    public TherapistExercisesControllerTests()
    {
        _dbOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: $"TherapistDb_{System.Guid.NewGuid()}")
            .Options;
    }

    [Fact]
    public async Task GetTherapistExercises_ReturnsListOfDtos()
    {
        // Arrange
        await using var context = new DataContext(_dbOptions);
        context.TherapistExercises.AddRange(new List<TherapistExercise>
        {
            new TherapistExercise
            {
                Name = "Neck Stretch",
                TargetArea = "Neck",
                SuitableFor = "Stiffness",
                Instructions = "Stretch slowly for 30 seconds",
                YoutubeUrl = "https://youtube.com/neck-stretch"
            },
            new TherapistExercise
            {
                Name = "Shoulder Roll",
                TargetArea = "Shoulders",
                SuitableFor = "Tension",
                Instructions = "Roll shoulders backward 10 times",
                YoutubeUrl = "https://youtube.com/shoulder-roll"
            }
        });
        await context.SaveChangesAsync();

        var userRepoMock = new Moq.Mock<API.Interfaces.IUserRepository>();
        var controller = new UsersController(userRepoMock.Object, context); // Pass mock repo and context

        // Act
        var result = await controller.getThrapistExercises();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var dtoList = Assert.IsType<List<TherapistExercisesDto>>(okResult.Value);
        Assert.Equal(2, dtoList.Count);

        Assert.Contains(dtoList, e => e.Name == "Neck Stretch" && e.TargetArea == "Neck");
        Assert.Contains(dtoList, e => e.Name == "Shoulder Roll" && e.TargetArea == "Shoulders");
    }
}
