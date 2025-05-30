using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Controllers;
using API.Data;
using API.Dtos;
using API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class TrainerSubscriptionsControllerTests
{
    private DataContext CreateContext() =>
        new(new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options);

    private ClaimsPrincipal CreateUser(string username) =>
        new(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, "mock"));

    [Fact]
    public async Task AddSubscription_ReturnsOk_WhenTrainer()
    {
        // Arrange
        var context = CreateContext();
        var trainerRole = new AppRole { Name = "Trainer" };
        var user = new AppUser
        {
            UserName = "trainer1",
            UserRoles = new List<AppUserRole> { new() { Role = trainerRole } }
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = new TrainerSubscriptionsController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser("trainer1") }
            }
        };

        var dto = new TrainerSubscriptionDto
        {
            Title = "Gold Plan",
            Type = "Monthly",
            Price = 49.99m
        };

        // Act
        var result = await controller.AddSubscription(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<TrainerSubscriptionDto>(okResult.Value);
        Assert.Equal(dto.Title, returned.Title);
        Assert.Equal(dto.Type, returned.Type);
        Assert.Equal(dto.Price, returned.Price);
    }

    [Fact]
    public async Task AddSubscription_ReturnsUnauthorized_WhenNotTrainer()
    {
        var context = CreateContext();
        var user = new AppUser
        {
            UserName = "nottrainer",
            UserRoles = new List<AppUserRole>() // no roles
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = new TrainerSubscriptionsController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser("nottrainer") }
            }
        };

        var dto = new TrainerSubscriptionDto { Title = "Plan", Type = "Weekly", Price = 20 };

        // Act
        var result = await controller.AddSubscription(dto);

        // Assert
        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        Assert.Equal("Only trainers can add subscriptions.", unauthorized.Value);
    }

    [Fact]
    public async Task AddSubscription_ReturnsUnauthorized_WhenNoUser()
    {
        var context = CreateContext();
        var controller = new TrainerSubscriptionsController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() // no user
            }
        };

        var dto = new TrainerSubscriptionDto { Title = "Plan", Type = "Yearly", Price = 100 };

        // Act
        var result = await controller.AddSubscription(dto);

        // Assert
        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task DeleteSubscription_Deletes_WhenValid()
    {
        var context = CreateContext();
        var trainer = new AppUser { UserName = "trainer", UserRoles = new List<AppUserRole>() };
        var sub = new TrainerSubscription
        {
            Title = "S1",
            Type = "Monthly",
            Price = 60,
            Trainer = trainer
        };
        context.Users.Add(trainer);
        context.TrainerSubscriptions.Add(sub);
        await context.SaveChangesAsync();

        var controller = new TrainerSubscriptionsController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser("trainer") }
            }
        };

        var result = await controller.DeleteSubscription(sub.Id);

        Assert.IsType<NoContentResult>(result);
        Assert.False(await context.TrainerSubscriptions.AnyAsync(x => x.Id == sub.Id));
    }

    [Fact]
    public async Task DeleteSubscription_ReturnsNotFound_WhenNotExists()
    {
        var context = CreateContext();
        context.Users.Add(new AppUser { UserName = "trainer" });
        await context.SaveChangesAsync();

        var controller = new TrainerSubscriptionsController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser("trainer") }
            }
        };

        var result = await controller.DeleteSubscription(999);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AddSubscription_ReturnsBadRequest_WhenModelInvalid()
    {
        var context = CreateContext();
        var trainerRole = new AppRole { Name = "Trainer" };
        var user = new AppUser
        {
            UserName = "trainer1",
            UserRoles = new List<AppUserRole> { new() { Role = trainerRole } }
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = new TrainerSubscriptionsController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser("trainer1") }
            }
        };

        controller.ModelState.AddModelError("Title", "Required");

        var dto = new TrainerSubscriptionDto
        {
            Title = null,
            Type = "Monthly",
            Price = 30
        };

        var result = await controller.AddSubscription(dto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.IsType<SerializableError>(badRequest.Value);
    }
}
