using API.Controllers;
using API.Data;
using API.Entities;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

public class ProfilePictureControllerTests
{
    private DataContext CreateContext() =>
        new(new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
            .Options);

    private ClaimsPrincipal CreateUser(string username) =>
        new(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }, "mock"));

    private IFormFile CreateFakeFile()
    {
        var fileMock = new Mock<IFormFile>();
        var content = "Fake image content";
        var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.Length).Returns(stream.Length);
        fileMock.Setup(f => f.FileName).Returns("test.jpg");
        fileMock.Setup(f => f.ContentType).Returns("image/jpeg");
        return fileMock.Object;
    }

    [Fact]
    public async Task UploadProfilePicture_AddsNewPicture_WhenNoneExists()
    {
        var context = CreateContext();
        var cloudMock = new Mock<ICloudinaryService>();
        cloudMock.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync("http://img.com/new.jpg");

        var user = new AppUser { UserName = "john" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = new ProfilePictureController(cloudMock.Object, context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser("john") }
            }
        };

        var result = await controller.UploadProfilePicture(CreateFakeFile());

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("imageUrl", ok.Value!.ToString());
    }

    [Fact]
    public async Task UploadProfilePicture_UpdatesExistingPicture()
    {
        var context = CreateContext();
        var cloudMock = new Mock<ICloudinaryService>();
        cloudMock.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>())).ReturnsAsync("http://img.com/updated.jpg");

        var user = new AppUser
        {
            UserName = "jane",
            ProfilePicture = new ProfilePicture { Url = "old.jpg" }
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = new ProfilePictureController(cloudMock.Object, context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser("jane") }
            }
        };

        var result = await controller.UploadProfilePicture(CreateFakeFile());

        var ok = Assert.IsType<OkObjectResult>(result);
        var updatedUser = await context.Users.Include(u => u.ProfilePicture).FirstOrDefaultAsync(u => u.UserName == "jane");
        Assert.Equal("http://img.com/updated.jpg", updatedUser!.ProfilePicture!.Url);
    }

    [Fact]
    public async Task UploadProfilePicture_ReturnsNotFound_WhenUserMissing()
    {
        var context = CreateContext();
        var cloudMock = new Mock<ICloudinaryService>();

        var controller = new ProfilePictureController(cloudMock.Object, context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser("ghost") }
            }
        };

        var result = await controller.UploadProfilePicture(CreateFakeFile());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task GetProfilePicture_ReturnsImageUrl_WhenExists()
    {
        var context = CreateContext();
        var user = new AppUser
        {
            UserName = "mike",
            ProfilePicture = new ProfilePicture { Url = "http://img.com/mike.jpg" }
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = new ProfilePictureController(new Mock<ICloudinaryService>().Object, context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser("mike") }
            }
        };

        var result = await controller.GetProfilePicture();

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Contains("http://img.com/mike.jpg", ok.Value!.ToString());
    }

    [Fact]
    public async Task GetProfilePicture_ReturnsNotFound_WhenNoPicture()
    {
        var context = CreateContext();
        var user = new AppUser { UserName = "nopic" };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = new ProfilePictureController(new Mock<ICloudinaryService>().Object, context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser("nopic") }
            }
        };

        var result = await controller.GetProfilePicture();

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Contains("No profile picture", notFound.Value!.ToString());
    }

    [Fact]
    public async Task DeleteProfilePicture_RemovesPicture()
    {
        var context = CreateContext();
        var user = new AppUser
        {
            UserName = "deletepic",
            ProfilePicture = new ProfilePicture { Url = "toDelete.jpg" }
        };
        context.Users.Add(user);
        await context.SaveChangesAsync();

        var controller = new ProfilePictureController(new Mock<ICloudinaryService>().Object, context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser("deletepic") }
            }
        };

        var result = await controller.DeleteProfilePicture();

        Assert.IsType<NoContentResult>(result);
        var updated = await context.Users.Include(u => u.ProfilePicture).FirstOrDefaultAsync(u => u.UserName == "deletepic");
        Assert.Null(updated!.ProfilePicture);
    }

    [Fact]
    public async Task DeleteProfilePicture_ReturnsNotFound_WhenUserMissing()
    {
        var context = CreateContext();

        var controller = new ProfilePictureController(new Mock<ICloudinaryService>().Object, context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = CreateUser("nouser") }
            }
        };

        var result = await controller.DeleteProfilePicture();

        Assert.IsType<NotFoundResult>(result);
    }
}
