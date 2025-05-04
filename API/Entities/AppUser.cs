using API.Extensions;
using Microsoft.AspNetCore.Identity;
namespace API.Entities;

public class AppUser : IdentityUser<int>
{
    
    public DateOnly DateOfBirth { get; set; }

    public int Weight { get; set; }

    public int Height { get; set; }

    public string? Gender { get; set; } 

    public ICollection<AppUserRole> UserRoles { get; set; } = [];

    public string? FitnessLevel { get; set; }

    public string? Traineegoal { get; set; }

    public string? MobileNumber { get; set; }   // New
    public string? Location { get; set; }       // New
    public string? GymName { get; set; }         // Only for Trainer    
    public ProfilePicture? ProfilePicture { get; set; }
    public ICollection<TrainerSubscription> TrainerSubscriptions { get; set; } = [];
    public string? ClinicName { get; set; } // For therapists
    public ICollection<TherapistSessionPrice> TherapistSessionPrices { get; set; } = [];




}
