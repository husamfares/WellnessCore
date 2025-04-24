using System;
using API.Data;
using API.Dtos;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController(IUserRepository userRepository , DataContext context) : BaseApiController
{

    [AllowAnonymous]
    [HttpGet] // api/users     (( will give you all users ))
    public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
    {
        var users = await userRepository.GetMembersAsync();


        return  Ok(users);
    }

 
    [HttpGet("{username}")] // api/users/ Alaa 
    public async Task<ActionResult<MemberDto>> GetUser(string username)
    {
        var user = await userRepository.GetMemberAsync(username);

        if(user == null) return NotFound();
        
        return user;

        
    }

    [HttpGet ("user/fitness-check")]
    public async Task<ActionResult<MemberDto>> GetFitnessCheckInfo()
    {
            var username = User.Identity?.Name;

            if (string.IsNullOrEmpty(username)) return Unauthorized("User not logged in");

            var user = await userRepository.GetMemberAsync(username);

            if (user == null) return NotFound("User not found");

            return Ok(user);


    }

    [HttpPut ("user/update-fitness-value")]
    
    public async Task<ActionResult> UpdateFitnessLevel(FitnessCheckUpdate fitnessCheckUpdate)
    {

        if(fitnessCheckUpdate.FitnessLevel == null || fitnessCheckUpdate.Traineegoal == null )
        {
            return BadRequest("no fitnessLevel or no goal");
        }
        var user = User.Identity?.Name;
        

        if (string.IsNullOrEmpty(user)) return Unauthorized("User not logged in");

        var username = await userRepository.GetUserByUsernameAsync(user);

        if(username == null) return BadRequest("there is no username inside DB");

        username.FitnessLevel = fitnessCheckUpdate.FitnessLevel;
        username.Traineegoal = fitnessCheckUpdate.Traineegoal;
        
    userRepository.Update(username); // optional if you're using tracking
    var result = await context.SaveChangesAsync();

    if (result > 0)
        return Ok(new { message = "Fitness level updated successfully" });
    else
        return BadRequest("something happen when update");

    
   // return BadRequest("Failed to update fitness level");
    }
}