using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using API.Controllers;
using API.Services;
using API.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using API.Entities;
using System.Security.Claims;
using Moq.Protected;
using API.Interfaces;

namespace WellnessCore.Tests.Controllers
{
    public class MealAnalyzerControllerTests
    {
        private readonly DbContextOptions<DataContext> _dbOptions;

        public MealAnalyzerControllerTests()
        {
            _dbOptions = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
        }

        [Fact]
        public async Task AnalyzeImage_ReturnsBadRequest_WhenNoImageProvided()
        {
            // Arrange
            var context = new DataContext(_dbOptions);
var cloudinaryMock = new Mock<ICloudinaryService>();
cloudinaryMock.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>()))
              .ReturnsAsync("https://example.com/fake.jpg");            var httpClientFactoryMock = new Mock<IHttpClientFactory>();

            var controller = new MealAnalyzerController(context, cloudinaryMock.Object, httpClientFactoryMock.Object);

            // Act
            var result = await controller.AnalyzeImage(null);

            // Assert
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("No image uploaded", badRequest.Value);
        }

        [Fact]
        public async Task AnalyzeImage_Returns500_WhenApiKeyMissing()
        {
            // Arrange
            var context = new DataContext(_dbOptions);
var cloudinaryMock = new Mock<ICloudinaryService>();
cloudinaryMock.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>()))
              .ReturnsAsync("https://example.com/fake.jpg");
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();

            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(100);

            Environment.SetEnvironmentVariable("OpenAIApiKey", null);

            var controller = new MealAnalyzerController(context, cloudinaryMock.Object, httpClientFactoryMock.Object);

            // Act
            var result = await controller.AnalyzeImage(mockFile.Object);

            // Assert
            var serverError = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverError.StatusCode);
        }

        [Fact]
        public async Task AnalyzeImage_ReturnsOk_WhenSuccess()
        {
            // Arrange
            var context = new DataContext(_dbOptions);

            var cloudinaryMock = new Mock<ICloudinaryService>();
            cloudinaryMock.Setup(x => x.UploadImageAsync(It.IsAny<IFormFile>()))
                          .ReturnsAsync("https://example.com/fake.jpg");

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(new
                    {
                        choices = new[] {
                            new {
                                message = new {
                                    content = "{ \"food\": \"Salad\", \"calories\": 150, \"protein_g\": 3.5 }"
                                }
                            }
                        }
                    }))
                });

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var mockFile = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("fake image data")), 0, 20, "image", "image.jpg");

            Environment.SetEnvironmentVariable("OpenAIApiKey", "fake-key");

            var controller = new MealAnalyzerController(context, cloudinaryMock.Object, httpClientFactoryMock.Object);

            // Mock user
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1")
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await controller.AnalyzeImage(mockFile);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            var meal = Assert.IsType<MealAnalysis>(ok.Value);
            Assert.Equal("Salad", meal.Food);
        }
    }
}
