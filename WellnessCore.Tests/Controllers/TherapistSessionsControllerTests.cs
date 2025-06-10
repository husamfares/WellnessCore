using API.Controllers;
using API.Data;
using API.Dtos;
using API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Xunit;

public class TherapistSessionsControllerTests
{
    private DataContext CreateContext() =>
        new(new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);

    private ClaimsPrincipal CreateUser(string username, bool isTherapist = false)
    {
        var claims = new List<Claim> { new(ClaimTypes.Name, username) };
        var identity = new ClaimsIdentity(claims, "mock");
        return new ClaimsPrincipal(identity);
    }

    [Fact]
    public async Task AddSession_ReturnsOk_WhenUserIsTherapist()
    {
        // Arrange
        var context = CreateContext();
        var therapistRole = new AppRole { Name = "Therapist" };
        var user = new AppUser
        {
            UserName = "therapist1",
            UserRoles = new List<AppUserRole>
            {
                new() { Role = therapistRole }
            }
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = new TherapistSessionsController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateUser("therapist1")
                }
            }
        };

        var dto = new TherapistSessionPriceDto
        {
            Title = "Session A",
            Price = 100
        };

        // Act
        var result = await controller.AddSession(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<TherapistSessionPriceDto>(okResult.Value);
        Assert.Equal("Session A", returned.Title);
        Assert.Equal(100, returned.Price);
    }

    [Fact]
    public async Task AddSession_ReturnsUnauthorized_WhenUserNotTherapist()
    {
        // Arrange
        var context = CreateContext();
        context.Users.Add(new AppUser
        {
            UserName = "user1",
            UserRoles = new List<AppUserRole>() // no roles
        });
        await context.SaveChangesAsync();

        var controller = new TherapistSessionsController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateUser("user1")
                }
            }
        };

        var dto = new TherapistSessionPriceDto { Title = "Test", Price = 50 };

        // Act
        var result = await controller.AddSession(dto);

        // Assert
        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        Assert.Equal("Only therapists can add session prices.", unauthorized.Value);
    }

    [Fact]
    public async Task DeleteSessionPrice_Deletes_WhenValidTherapist()
    {
        // Arrange
        var context = CreateContext();
        var therapist = new AppUser { UserName = "therapist", UserRoles = new List<AppUserRole>() };
        var session = new TherapistSessionPrice
        {
            Title = "S1",
            Price = 99,
            Therapist = therapist
        };
        context.Users.Add(therapist);
        context.TherapistSessionPrices.Add(session);
        await context.SaveChangesAsync();

        var controller = new TherapistSessionsController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateUser("therapist")
                }
            }
        };

        // Act
        var result = await controller.DeleteSessionPrice(session.Id);

        // Assert
        Assert.IsType<NoContentResult>(result);
        Assert.False(await context.TherapistSessionPrices.AnyAsync(x => x.Id == session.Id));
    }

    [Fact]
    public async Task DeleteSessionPrice_ReturnsNotFound_WhenSessionNotExists()
    {
        // Arrange
        var context = CreateContext();
        context.Users.Add(new AppUser { UserName = "therapist", UserRoles = new List<AppUserRole>() });
        await context.SaveChangesAsync();

        var controller = new TherapistSessionsController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateUser("therapist")
                }
            }
        };

        // Act
        var result = await controller.DeleteSessionPrice(999); // non-existing ID

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AddSession_ReturnsUnauthorized_WhenNoUser()
    {
        // Arrange
        var context = CreateContext();

        var controller = new TherapistSessionsController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() // No user in context
            }
        };

        var dto = new TherapistSessionPriceDto { Title = "Test", Price = 50 };

        // Act
        var result = await controller.AddSession(dto);

        // Assert
        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        Assert.Equal("Only therapists can add session prices.", unauthorized.Value);
    }

[Fact]
public async Task AddSession_ReturnsBadRequest_WhenModelIsInvalid()
{
    // Arrange
    var context = CreateContext();
    var therapistRole = new AppRole { Name = "Therapist" };
    var user = new AppUser
    {
        UserName = "therapist1",
        UserRoles = new List<AppUserRole> { new() { Role = therapistRole } }
    };
    context.Users.Add(user);
    await context.SaveChangesAsync();

    var controller = new TherapistSessionsController(context)
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = CreateUser("therapist1")
            }
        }
    };

    // Simulate model state error
    controller.ModelState.AddModelError("Price", "Price must be positive");

    var invalidDto = new TherapistSessionPriceDto
    {
        Title = "Bad Session",
        Price = -10
    };

    // Act
    var result = await controller.AddSession(invalidDto);

    // Assert
    var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
    Assert.IsType<SerializableError>(badRequest.Value);
}


}
