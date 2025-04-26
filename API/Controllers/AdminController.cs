using System;
using API.Dtos;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(UserManager<AppUser> userManager) : BaseApiController
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await userManager.Users.ToListAsync();

    var result = new List<object>();

    foreach (var user in users)
    {
        // Ensure each GetRolesAsync runs sequentially
        var roles = await userManager.GetRolesAsync(user);

        result.Add(new {
            user.Id,
            user.UserName,
            Roles = roles
        });
    }

    return Ok(result);
    }

    [HttpPost("edit-roles/{username}")]
    public async Task<IActionResult> EditRoles(string username, [FromQuery] string roles)
    {
         var selectedRoles = roles.Split(",").ToArray();
        var user = await userManager.FindByNameAsync(username);

        if (user == null) return NotFound("User not found");

        var userRoles = await userManager.GetRolesAsync(user);
        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        if (!result.Succeeded) return BadRequest("Failed to add roles");

        result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        if (!result.Succeeded) return BadRequest("Failed to remove roles");

        return Ok(await userManager.GetRolesAsync(user));
    }
}


