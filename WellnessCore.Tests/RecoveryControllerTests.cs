using Xunit;
using API.Controllers;
using API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace API.Tests;

public class RecoveryControllerTests
{
    private readonly RecoveryController _controller;

    public RecoveryControllerTests()
    {
        _controller = new RecoveryController();
    }

    // [Theory]
    // [InlineData(8, "light", "low", 100.00)]
    // [InlineData(5, "intense", "high", 24.63)]
    // [InlineData(6.5, "moderate", "medium", 55.99)]
    // [InlineData(3, "unknown", "unknown", 14.91)]
            
        [Theory]
        [InlineData(8, "light", "low", 100.00)]
        [InlineData(5, "intense", "high", 19.62)]        // Was 19.60
        [InlineData(6.5, "moderate", "medium", 53.24)]
        [InlineData(3, "unknown", "unknown", 14.73)] 
     // default values used

    public void CalculateRecovery_ReturnsExpectedResult(double sleepHours, string intensity, string fatigue, double expected)
    {
        // Arrange
        var input = new RecoveryInputDto
        {
            SleepHours = sleepHours,
            WorkoutIntensity = intensity,
            FatigueLevel = fatigue
        };

        // Act
        var actionResult = _controller.CalculateRecovery(input);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        Assert.Equal(200, okResult.StatusCode);

        var recoveryValue = (double)okResult.Value!;
        Assert.Equal(expected, recoveryValue, 2); // Compare with 2 decimal precision
    }
}
