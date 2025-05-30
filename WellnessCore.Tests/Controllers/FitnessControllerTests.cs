using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Controllers;
using API.Entities;
using API.Interfaces;
using API.Data;
using API.Dtos;

public class FitnessControllerTests
{
    private readonly DbContextOptions<DataContext> _dbOptions;

    public FitnessControllerTests()
    {
        _dbOptions = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;
    }

    [Fact]
    public async Task UpdateFitnessLevel_ValidData_UpdatesUserAndReturnsOk()
    {
        // Arrange
        var username = "testuser";
        var user = new AppUser
        {
            UserName = username,
            FitnessLevel = null,
            Traineegoal = null
        };

        await using var context = new DataContext(_dbOptions);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(repo => repo.GetUserByUsernameAsync(username))
            .ReturnsAsync(user);

        var controller = new UsersController(userRepositoryMock.Object, context);
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, username)
                }))
            }
        };

        var dto = new FitnessCheckUpdate
        {
            FitnessLevel = "Advanced",
            Traineegoal = "Build Muscle"
        };

        // Act
        var result = await controller.UpdateFitnessLevel(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);

        var updatedUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        Assert.NotNull(updatedUser);
        Assert.Equal("Advanced", updatedUser.FitnessLevel);
        Assert.Equal("Build Muscle", updatedUser.Traineegoal);
    }
}
