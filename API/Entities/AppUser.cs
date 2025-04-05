using API.Extensions;
namespace API.Entities;

public class AppUser
{
    public int Id { get; set; }
    public required string UserName { get; set; }
    public required byte[] PasswordHash { get; set; } = [];
    public required byte[] PasswordSalt { get; set; } = [];


    public DateOnly? DateOfBirth { get; set; }  // nullable

    public string? Weight { get; set; }

    public string? Height { get; set; }

    public string? Gender { get; set; } 

    // public int GetAge()
    // {
    //     return DateOfBirth.CalculateAge();
    // }
}
