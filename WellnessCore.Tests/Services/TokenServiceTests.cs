using Xunit;
using Moq;
using API.Services;
using API.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

public class TokenServiceTests
{
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<UserManager<AppUser>> _mockUserManager;
    private readonly TokenService _tokenService;
    private readonly string validTokenKey;

    public TokenServiceTests()
    {
        validTokenKey = new string('A', 64); // Valid 64-character token key

        _mockConfig = new Mock<IConfiguration>();
        _mockConfig.Setup(c => c["TokenKey"]).Returns(validTokenKey);

        var userStore = new Mock<IUserStore<AppUser>>();
        _mockUserManager = new Mock<UserManager<AppUser>>(
            userStore.Object,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!,
            null!
        );

        _tokenService = new TokenService(_mockConfig.Object, _mockUserManager.Object);
    }

    [Fact]
    public async Task CreateToken_ReturnsValidJwtToken()
    {
        // Arrange
        var user = new AppUser { Id = 1, UserName = "testuser" };
        var roles = new List<string> { "User", "Admin" };

        _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);

        // Act
        var token = await _tokenService.CreateToken(user);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public async Task CreateToken_ThrowsException_WhenTokenKeyIsMissing()
    {
        // Arrange
        _mockConfig.Setup(c => c["TokenKey"]).Returns((string?)null);
        var service = new TokenService(_mockConfig.Object, _mockUserManager.Object);
        var user = new AppUser { Id = 1, UserName = "testuser" };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => service.CreateToken(user));
        Assert.Equal("Cannot access tokenKey from appsetting", ex.Message);
    }

    [Fact]
    public async Task CreateToken_ThrowsException_WhenTokenKeyTooShort()
    {
        // Arrange
        _mockConfig.Setup(c => c["TokenKey"]).Returns("shortkey");
        var service = new TokenService(_mockConfig.Object, _mockUserManager.Object);
        var user = new AppUser { Id = 1, UserName = "testuser" };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => service.CreateToken(user));
        Assert.Equal("Your token key needs to be longer", ex.Message);
    }

    [Fact]
    public async Task CreateToken_ThrowsException_WhenUsernameIsNull()
    {
        // Arrange
        var user = new AppUser { Id = 1, UserName = null };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<Exception>(() => _tokenService.CreateToken(user));
        Assert.Equal("UserName is null", ex.Message); // 🔧 Fixed message (removed !)
    }

   [Fact]
public async Task CreateToken_IncludesUserClaimsAndRoles()
{
    // Arrange
    var user = new AppUser { Id = 123, UserName = "claimuser" };
    var roles = new List<string> { "Role1", "Role2" };

    _mockUserManager.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);

    // Act
    var token = await _tokenService.CreateToken(user);
    var handler = new JwtSecurityTokenHandler();
    var jwt = handler.ReadJwtToken(token);

    // Assert
    Assert.Contains(jwt.Claims, c => c.Type == "nameid" && c.Value == "123");
    Assert.Contains(jwt.Claims, c => c.Type == "unique_name" && c.Value == "claimuser");
    Assert.Contains(jwt.Claims, c => c.Type == "role" && c.Value == "Role1");
    Assert.Contains(jwt.Claims, c => c.Type == "role" && c.Value == "Role2");
}
}
