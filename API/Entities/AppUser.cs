namespace API.Entities;

public class AppUser
{
    public int Id { get; set; }
    public required string UserName { get; set; }
    public required byte[] PasswordHash { get; set; }
    public required byte[] PasswordSalt { get; set; }

    public int Weight { get; set; }

    public int Height { get; set; }

    public string? Gender { get; set; } 

    public int Age { get; set; }
}
