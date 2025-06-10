using API.Controllers;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

public class ProfileControllerTests
{
    private DataContext CreateContext() =>
        new(new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options);

    private ClaimsPrincipal CreateUser(string username) =>
        new(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, "mock"));

    [Fact]
    public async Task GetProfile_ReturnsProfile_WhenExists()
    {
        var context = CreateContext();
        var user = new AppUser
        {
            UserName = "user1",
            MobileNumber = "123456",
            Location = "City",
            GymName = "Gold Gym",
            ClinicName = "Wellness Clinic",
            ProfilePicture = new ProfilePicture { Url = "http://img.com/1.jpg" },
            UserRoles = [new AppUserRole { Role = new AppRole { Name = "Trainer" } }],
            TrainerSubscriptions = [new TrainerSubscription { Title = "Plan A", Type = "Monthly", Price = 50 }],
            TherapistSessionPrices = [new TherapistSessionPrice { Title = "Session A", Price = 80 }]
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = new ProfileController(context);

        var result = await controller.GetProfile("user1");

        var ok = Assert.IsType<ActionResult<ProfileDto>>(result);
        var dto = Assert.IsType<ProfileDto>(ok.Value);
        Assert.Equal("user1", dto.Username);
        Assert.Equal("Trainer", dto.Role);
        Assert.Single(dto.Subscriptions);
        Assert.Single(dto.SessionPrices);
    }

    [Fact]
    public async Task GetProfile_ReturnsNotFound_WhenMissing()
    {
        var context = CreateContext();
        var controller = new ProfileController(context);

        var result = await controller.GetProfile("notfound");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task UpdateProfile_UpdatesFields_WhenUserExists()
    {
        var context = CreateContext();
        var user = new AppUser
        {
            UserName = "edituser",
            MobileNumber = "000",
            Location = "Old",
            GymName = "G",
            ClinicName = "C"
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = new ProfileController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateUser("edituser")
                }
            }
        };

        var dto = new ProfileDto
        {
            Username = "edituser",
            Role = "TestRole",
            MobileNumber = "999",
            Location = "New City",
            GymName = "New Gym",
            ClinicName = "New Clinic"
        };

        var result = await controller.UpdateProfile(dto);

        Assert.IsType<NoContentResult>(result);

        var updated = await context.Users.FirstAsync(u => u.UserName == "edituser");
        Assert.Equal("999", updated.MobileNumber);
        Assert.Equal("New City", updated.Location);
        Assert.Equal("New Gym", updated.GymName);
        Assert.Equal("New Clinic", updated.ClinicName);
    }

    [Fact]
    public async Task UpdateProfile_ReturnsNotFound_WhenUserMissing()
    {
        var context = CreateContext();
        var controller = new ProfileController(context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = CreateUser("missinguser")
                }
            }
        };

        var dto = new ProfileDto { Username = "missinguser", Role = "TestRole", MobileNumber = "1", Location = "L", GymName = "G", ClinicName = "C" };

        var result = await controller.UpdateProfile(dto);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetAllProfiles_ReturnsAll_WhenNoFilter()
    {
        var context = CreateContext();
        context.Users.AddRange(
            new AppUser
            {
                UserName = "a",
                PhoneNumber = "111",
                Location = "Loc1",
                GymName = "Gym A",
                UserRoles = [new AppUserRole { Role = new AppRole { Name = "Trainer" } }]
            },
            new AppUser
            {
                UserName = "b",
                PhoneNumber = "222",
                Location = "Loc2",
                GymName = "Gym B",
                UserRoles = [new AppUserRole { Role = new AppRole { Name = "Therapist" } }]
            }
        );
        await context.SaveChangesAsync();

        var controller = new ProfileController(context);

        var result = await controller.GetAllProfiles();

        var ok = Assert.IsType<ActionResult<IEnumerable<ProfileDto>>>(result);
        var profiles = Assert.IsType<List<ProfileDto>>(ok.Value);
        Assert.Equal(2, profiles.Count);
    }

    [Fact]
    public async Task GetAllProfiles_ReturnsFiltered_ByRole()
    {
        var context = CreateContext();
        context.Users.AddRange(
            new AppUser
            {
                UserName = "trainer",
                PhoneNumber = "111",
                Location = "Loc",
                UserRoles = [new AppUserRole { Role = new AppRole { Name = "Trainer" } }]
            },
            new AppUser
            {
                UserName = "client",
                PhoneNumber = "222",
                Location = "Loc",
                UserRoles = [new AppUserRole { Role = new AppRole { Name = "Client" } }]
            }
        );
        await context.SaveChangesAsync();

        var controller = new ProfileController(context);

        var result = await controller.GetAllProfiles("Trainer");

        var ok = Assert.IsType<ActionResult<IEnumerable<ProfileDto>>>(result);
        var profiles = Assert.IsType<List<ProfileDto>>(ok.Value);
        Assert.Single(profiles);
        Assert.Equal("trainer", profiles[0].Username);
    }
}
