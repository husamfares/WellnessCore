using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public static class Seed
{
    public static async Task SeedRolesAsync(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        var roles = new[] { "Admin", "Trainer", "Therapist", "User" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new AppRole { Name = role });
        }

        if (await userManager.Users.AnyAsync()) return;

         var admin = new AppUser
        {
            UserName = "admin",
            Gender = "Male",
            DateOfBirth = new DateOnly(1990, 1, 1),
            Height = 180,
            Weight = 80
        };

        // Set admin password
        var result = await userManager.CreateAsync(admin, "Pa$$w0rd");

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
