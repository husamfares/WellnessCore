using System.ComponentModel.DataAnnotations;

namespace API.Dtos;

public class RegisterDto
{
    [Required]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [StringLength(8, MinimumLength = 4)]
    public string Password { get; set; } = string.Empty;


    public DateOnly DateOfBirth { get; set; }

    public string? Weight { get; set; }

    public string ? Height { get; set; }

    public string? Gender { get; set; } 
}
