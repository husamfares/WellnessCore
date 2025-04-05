using System.ComponentModel.DataAnnotations;

namespace API.Dtos;

public class RegisterDto
{
    [Required]
    public string Username { get; set; } =string.Empty;
    
    [Required]
    public string? DateOfBirth { get; set; }
    
    [Required]
    public int Weight { get; set; }
    
    [Required]
    public int Height { get; set; }

    [Required]
    public string? Gender { get; set; } 

    [Required]
    [StringLength(8, MinimumLength = 4)]
    public string Password { get; set; } =string.Empty;
}
