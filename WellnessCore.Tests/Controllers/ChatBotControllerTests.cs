using API.Controllers;
using API.Data;
using API.Dtos;
using API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace WellnessCore.Tests.Controllers;

public class ChatbotControllerTests
{
    private readonly DataContext _context;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;

    public ChatbotControllerTests()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new DataContext(options);
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();

        // Seed a test user (no hardcoded Id!)
        var user = new AppUser
        {
            UserName = "testuser",
            Traineegoal = "lose weight",
            Gender = "male",
            Weight = 80,
            Height = 180
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        // Simulate logged-in user with same ID
        var userId = user.Id.ToString();
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims, "mock");
        var principal = new ClaimsPrincipal(identity);

        var httpContext = new DefaultHttpContext { User = principal };
        var controllerContext = new ControllerContext { HttpContext = httpContext };

        Controller = new ChatbotController(_context, _httpClientFactoryMock.Object)
        {
            ControllerContext = controllerContext
        };
    }

    public ChatbotController Controller { get; }

    [Fact]
    public async Task ChatWithCoach_ReturnsAssistantMessage()
    {
        // Arrange
        var responseJson = """
        {
            "choices": [
                {
                    "message": {
                        "content": "Here is a basic workout plan..."
                    }
                }
            ]
        }
        """;

        var handler = new Mock<HttpMessageHandler>();
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            });

        var client = new HttpClient(handler.Object);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        var userMessage = new ChatMessageDto
        {
            Role = "user",
            Content = "Give me a workout"
        };

        // Act
        var result = await Controller.ChatWithCoach(userMessage);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ChatResponseDto>(okResult.Value);
        Assert.Single(response.Messages);
        Assert.Equal("assistant", response.Messages[0].Role);
        Assert.Contains("workout", response.Messages[0].Content, StringComparison.OrdinalIgnoreCase);
    }
}
