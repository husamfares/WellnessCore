// API/DTOs/ProfileDto.cs
using API.Dtos;

namespace API.DTOs;

public class ProfileDto
{
    public required string Username { get; set; }
    public required string Role { get; set; }
    public string? MobileNumber { get; set; }
    public string? Location { get; set; }
    public string? GymName { get; set; } // for Trainers
    public string? ClinicName { get; set; } // for Therapists
    public string? ProfilePictureUrl { get; set; }
    public List<TrainerSubscriptionDto> Subscriptions { get; set; } = new();
    public List<TherapistSessionPriceDto> SessionPrices { get; set; } = new();

}
