using Microsoft.AspNetCore.Mvc;
using API.Services;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfilePictureController : ControllerBase
    {
        private readonly CloudinaryService _cloudinaryService;
        private readonly UserManager<AppUser> _userManager;
        private readonly DataContext _context;

        public ProfilePictureController(
            CloudinaryService cloudinaryService,
            UserManager<AppUser> userManager,
            DataContext context)
        {
            _cloudinaryService = cloudinaryService;
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadProfilePicture([FromForm] IFormFile file)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var existingProfilePicture = await _context.ProfilePictures
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            if (existingProfilePicture != null)
            {
                _context.ProfilePictures.Remove(existingProfilePicture);
                await _context.SaveChangesAsync();
            }

            var imageUrl = await _cloudinaryService.UploadImageAsync(file);

            var profilePicture = new ProfilePicture
            {
                Url = imageUrl,
                UserId = user.Id
            };

            user.ProfilePicture = profilePicture;
            await _context.SaveChangesAsync();

            return Ok(new { imageUrl });
        }

        [HttpGet]
        public async Task<IActionResult> GetProfilePicture()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            await _context.Entry(user).Reference(u => u.ProfilePicture).LoadAsync();

            if (user.ProfilePicture == null)
                return NotFound(new { message = "No profile picture found." });

            return Ok(new { imageUrl = user.ProfilePicture.Url });
        }
    }
}
