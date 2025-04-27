// API/DTOs/ProfileDto.cs
namespace API.DTOs;

public class ProfileDto
{
    public string? Username { get; set; }
    public string? MobileNumber { get; set; }
    public string? Location { get; set; }
    public string? GymName { get; set; }
    public required string Role { get; set; }
}
