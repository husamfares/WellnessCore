using API.Controllers;
using API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Moq;
using static WellnessCore.Tests.Helpers.TsetAsyncHelper;


public class AdminControllerTests
{
    [Fact]
    public async Task GetUsersWithRoles_ReturnsUsersWithRoles()
    {
        // Arrange
        var users = new List<AppUser>
        {
            new() { Id = 1, UserName = "admin1" },
            new() { Id = 2, UserName = "user2" }
        };

        var userManager = MockUserManager.CreateMock<AppUser>();
        userManager.Setup(um => um.Users).Returns(new TestAsyncEnumerable<AppUser>(users));
        userManager.Setup(um => um.GetRolesAsync(It.IsAny<AppUser>())).ReturnsAsync((AppUser u) =>
            u.UserName == "admin1" ? new List<string> { "Admin" } : new List<string> { "Member" });

        var controller = new AdminController(userManager.Object);

        // Act
        var result = await controller.GetUsersWithRoles();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var list = Assert.IsType<List<object>>(ok.Value);
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public async Task EditRoles_ReturnsOk_WhenRolesUpdatedSuccessfully()
    {
        // Arrange
        var user = new AppUser { UserName = "testuser" };
        var userManager = MockUserManager.CreateMock<AppUser>();
        userManager.Setup(um => um.FindByNameAsync("testuser")).ReturnsAsync(user);
        userManager.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Member" });

        userManager.Setup(um => um.AddToRolesAsync(user, It.IsAny<IEnumerable<string>>()))
                   .ReturnsAsync(IdentityResult.Success);

        userManager.Setup(um => um.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
                   .ReturnsAsync(IdentityResult.Success);

        userManager.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Admin", "User" });

        var controller = new AdminController(userManager.Object);

        // Act
        var result = await controller.EditRoles("testuser", "Admin,User");

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var roles = Assert.IsAssignableFrom<IEnumerable<string>>(ok.Value);
        Assert.Contains("Admin", roles);
        Assert.Contains("User", roles);
    }

    [Fact]
    public async Task EditRoles_ReturnsNotFound_WhenUserNotFound()
    {
        var userManager = MockUserManager.CreateMock<AppUser>();
        userManager.Setup(um => um.FindByNameAsync("missing")).ReturnsAsync((AppUser)null!);

        var controller = new AdminController(userManager.Object);

        var result = await controller.EditRoles("missing", "Admin");

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("User not found", notFound.Value);
    }

    [Fact]
    public async Task EditRoles_ReturnsBadRequest_WhenAddFails()
    {
        var user = new AppUser { UserName = "testuser" };
        var userManager = MockUserManager.CreateMock<AppUser>();
        userManager.Setup(um => um.FindByNameAsync("testuser")).ReturnsAsync(user);
        userManager.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { });

        userManager.Setup(um => um.AddToRolesAsync(user, It.IsAny<IEnumerable<string>>()))
                   .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Add failed" }));

        var controller = new AdminController(userManager.Object);

        var result = await controller.EditRoles("testuser", "Admin");

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Failed to add roles", bad.Value);
    }

    [Fact]
    public async Task EditRoles_ReturnsBadRequest_WhenRemoveFails()
    {
        var user = new AppUser { UserName = "testuser" };
        var userManager = MockUserManager.CreateMock<AppUser>();
        userManager.Setup(um => um.FindByNameAsync("testuser")).ReturnsAsync(user);
        userManager.Setup(um => um.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Member" });

        userManager.Setup(um => um.AddToRolesAsync(user, It.IsAny<IEnumerable<string>>()))
                   .ReturnsAsync(IdentityResult.Success);

        userManager.Setup(um => um.RemoveFromRolesAsync(user, It.IsAny<IEnumerable<string>>()))
                   .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Remove failed" }));

        var controller = new AdminController(userManager.Object);

        var result = await controller.EditRoles("testuser", "Admin");

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Failed to remove roles", bad.Value);
    }
}
