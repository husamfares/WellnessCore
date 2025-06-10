using System;
using System.Security.Claims;
using API.Data;
using API.Dtos;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class TrainerSubscriptionsController(DataContext context) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<TrainerSubscriptionDto>> AddSubscription(TrainerSubscriptionDto dto)
    {
        if (!ModelState.IsValid)
        return BadRequest(ModelState);


        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        var user = await context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(x => x.UserName == username);


        if (user == null || !user.UserRoles.Any(r => r.Role.Name == "Trainer"))
            return Unauthorized("Only trainers can add subscriptions.");

        var sub = new TrainerSubscription
        {
            Title = dto.Title,
            Type = dto.Type,
            Price = dto.Price,
            TrainerId = user.Id
        };

        context.TrainerSubscriptions.Add(sub);
        await context.SaveChangesAsync();

        return Ok(new TrainerSubscriptionDto
        {
            Title = sub.Title,
            Type = sub.Type,
            Price = sub.Price
        });
    }

    [HttpDelete("{id}")]
public async Task<IActionResult> DeleteSubscription(int id)
{
    var username = User.FindFirst(ClaimTypes.Name)?.Value;
    var user = await context.Users.Include(u => u.UserRoles)
                                  .FirstOrDefaultAsync(x => x.UserName == username);

    if (user == null) return Unauthorized();

    var sub = await context.TrainerSubscriptions.FirstOrDefaultAsync(x => x.Id == id && x.TrainerId == user.Id);
    if (sub == null) return NotFound();

    context.TrainerSubscriptions.Remove(sub);
    await context.SaveChangesAsync();

    return NoContent();
}

}
