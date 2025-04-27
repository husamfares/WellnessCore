// API/Controllers/ProfileController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    public class ProfileController : BaseApiController
    {
        private readonly DataContext _context;

        public ProfileController(DataContext context)
        {
            _context = context;
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<ProfileDto>> GetProfile(string username)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(r => r.Role)
                .FirstOrDefaultAsync(x => x.UserName == username);

            if (user == null) return NotFound();

            var role = user.UserRoles.FirstOrDefault()?.Role.Name ?? "User";

            return new ProfileDto
            {
                Username = user.UserName,
                MobileNumber = user.MobileNumber,
                Location = user.Location,
                GymName = user.GymName,
                Role = role
            };
        }

        [HttpPut]
        public async Task<ActionResult> UpdateProfile(ProfileDto profileDto)
        {
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == username);

            if (user == null) return NotFound();

            user.MobileNumber = profileDto.MobileNumber;
            user.Location = profileDto.Location;
            user.GymName = profileDto.GymName;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet]
public async Task<ActionResult<IEnumerable<ProfileDto>>> GetAllProfiles([FromQuery] string? role = null)
{
    var usersQuery = _context.Users
        .Include(u => u.UserRoles)
        .ThenInclude(r => r.Role)
        .AsQueryable();

    if (!string.IsNullOrEmpty(role))
    {
        usersQuery = usersQuery.Where(u => u.UserRoles.Any(r => r.Role.Name == role));
    }

    var users = await usersQuery.ToListAsync();

    var profiles = users.Select(user => new ProfileDto
    {
        Username = user.UserName,
        MobileNumber = user.PhoneNumber,
        Location = user.Location,
        GymName = user.GymName,
        Role = user.UserRoles.FirstOrDefault()?.Role.Name ?? "User"
    }).ToList();

    return profiles;
}
    }
}
