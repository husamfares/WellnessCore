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

}
