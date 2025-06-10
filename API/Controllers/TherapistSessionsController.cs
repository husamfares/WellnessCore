using System.Security.Claims;
using API.Data;
using API.Dtos;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class TherapistSessionsController(DataContext context) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<TherapistSessionPriceDto>> AddSession(TherapistSessionPriceDto dto)
    {
         if (!ModelState.IsValid)
        return BadRequest(ModelState);

        var username = User.FindFirst(ClaimTypes.Name)?.Value;

        var user = await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(r => r.Role)
            .FirstOrDefaultAsync(x => x.UserName == username);

        if (user == null || !user.UserRoles.Any(r => r.Role.Name == "Therapist"))
            return Unauthorized("Only therapists can add session prices.");

        var session = new TherapistSessionPrice
        {
            Title = dto.Title!,
            Price = dto.Price,
            TherapistId = user.Id
        };

        context.TherapistSessionPrices.Add(session);
        await context.SaveChangesAsync();

        return Ok(new TherapistSessionPriceDto
        {
            Title = session.Title,
            Price = session.Price
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSessionPrice(int id)
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        var user = await context.Users.Include(u => u.UserRoles)
                                      .FirstOrDefaultAsync(x => x.UserName == username);

        if (user == null) return Unauthorized();

        var session = await context.TherapistSessionPrices.FirstOrDefaultAsync(x => x.Id == id && x.TherapistId == user.Id);
        if (session == null) return NotFound();

        context.TherapistSessionPrices.Remove(session);
        await context.SaveChangesAsync();

        return NoContent();
    }



}
