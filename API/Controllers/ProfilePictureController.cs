using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using API.Interfaces;

namespace API.Controllers
{
    [Authorize]
    public class ProfilePictureController(ICloudinaryService cloudinaryService, DataContext context) : BaseApiController
    {


         [HttpPost("upload")]
    public async Task<IActionResult> UploadProfilePicture([FromForm] IFormFile file)
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;

        var user = await context.Users
            .Include(u => u.ProfilePicture)
            .FirstOrDefaultAsync(x => x.UserName == username);

        if (user == null) return NotFound();

        var imageUrl = await cloudinaryService.UploadImageAsync(file);

        // Update existing or add new profile picture
        if (user.ProfilePicture != null)
        {
            user.ProfilePicture.Url = imageUrl;
        }
        else
        {
            user.ProfilePicture = new ProfilePicture
            {
                Url = imageUrl,
                UserId = user.Id
            };
        }

        await context.SaveChangesAsync();

        return Ok(new { imageUrl });
    }

    [HttpGet]
    public async Task<IActionResult> GetProfilePicture()
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        var user = await context.Users.Include(u => u.ProfilePicture)
            .FirstOrDefaultAsync(x => x.UserName == username);

        if (user == null) return NotFound();

        if (user.ProfilePicture == null)
            return NotFound(new { message = "No profile picture found." });

        return Ok(new { imageUrl = user.ProfilePicture.Url });
    }

    [HttpDelete]
public async Task<IActionResult> DeleteProfilePicture()
{
    var username = User.FindFirst(ClaimTypes.Name)?.Value;

    var user = await context.Users
        .Include(u => u.ProfilePicture)
        .FirstOrDefaultAsync(x => x.UserName == username);

    if (user == null || user.ProfilePicture == null)
        return NotFound();

    // Optional: Delete from Cloudinary if you want (I'll give the code if you want)
    // await cloudinaryService.DeleteImageAsync(user.ProfilePicture.PublicId);

    context.ProfilePictures.Remove(user.ProfilePicture);
    user.ProfilePicture = null;

    await context.SaveChangesAsync();

    return NoContent();
}
}
}
