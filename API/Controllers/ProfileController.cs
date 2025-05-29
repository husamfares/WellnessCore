// API/Controllers/ProfileController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using API.Dtos;

namespace API.Controllers
{
    [Authorize]
    public class ProfileController(DataContext context) : BaseApiController
    {


        [HttpGet("{username}")]
        public async Task<ActionResult<ProfileDto>> GetProfile(string username)
        {
            var user = await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(r => r.Role)
            .Include(u => u.ProfilePicture)
            .Include(u => u.TrainerSubscriptions)
            .Include(u => u.TherapistSessionPrices)
            .FirstOrDefaultAsync(x => x.UserName == username);

            if (user == null) return NotFound();

            var role = user.UserRoles.FirstOrDefault()?.Role.Name ?? "User";

            return new ProfileDto
            {
                Username = user.UserName!,
                MobileNumber = user.MobileNumber,
                Location = user.Location,
                GymName = user.GymName,
                ClinicName = user.ClinicName,
                Role = role,
                ProfilePictureUrl = user.ProfilePicture?.Url,
                Subscriptions = user.TrainerSubscriptions.Select(s => new TrainerSubscriptionDto
                {
                    Id = s.Id,
                    Type = s.Type,
                    Title = s.Title,
                    Price = s.Price
                }).ToList(),
                SessionPrices = user.TherapistSessionPrices.Select(p => new TherapistSessionPriceDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Price = p.Price
                }).ToList()
            };
        }
        [HttpPut]
        public async Task<ActionResult> UpdateProfile(ProfileDto profileDto)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == username);

            if (user == null) return NotFound();

            user.MobileNumber = profileDto.MobileNumber;
            user.Location = profileDto.Location;
            user.GymName = profileDto.GymName;
            user.ClinicName = profileDto.ClinicName; // 🔥 Add this line

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProfileDto>>> GetAllProfiles([FromQuery] string? role = null)
        {
            var usersQuery = context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(r => r.Role)
                .Include(u => u.ProfilePicture)
                .AsQueryable();

            if (!string.IsNullOrEmpty(role))
            {
                usersQuery = usersQuery.Where(u => u.UserRoles.Any(r => r.Role.Name == role));
            }

            var users = await usersQuery.ToListAsync();

            var profiles = users.Select(user => new ProfileDto
            {
                Username = user.UserName!,
                MobileNumber = user.PhoneNumber,
                Location = user.Location,
                GymName = user.GymName,
                Role = user.UserRoles.FirstOrDefault()?.Role.Name ?? "User",
                ProfilePictureUrl = user.ProfilePicture?.Url
            }).ToList();

            return profiles;
        }
    }
}
