using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Dtos;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(UserManager<AppUser> userManager, ItokenService tokenService, IMapper mapper) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await UserExists(registerDto.Username))
        {
            return BadRequest("Username is taken");
        }

        if (registerDto.Username == null || registerDto.Password == null || registerDto.Username == "" || registerDto.Password == "")
        {
            return BadRequest("username or password cant be empty");
        }

        var user = mapper.Map<AppUser>(registerDto);

        user.UserName = registerDto.Username.ToLower();

        var result = await userManager.CreateAsync(user, registerDto.Password);

        if(!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        return Ok(new UserDto
        {
            Username = user.UserName,
            Gender = user.Gender!,
            Token = await tokenService.CreateToken(user),
        });

    }
    // Login

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto logingDto)
    {
        var user = await userManager.Users.FirstOrDefaultAsync(x => x.NormalizedUserName == logingDto.Username.ToUpper());

        if (user == null || user.UserName == null) return Unauthorized("Invalid username");

        var result = await userManager.CheckPasswordAsync(user, logingDto.Password);

        if(!result) return Unauthorized("Invalid password");

        return new UserDto
        {
            Username = user.UserName,
            Gender = user.Gender!,
            Token = await tokenService.CreateToken(user)

        };
    }


    private async Task<bool> UserExists(string username)
    {
        return await userManager.Users.AnyAsync(x => x.NormalizedUserName == username.ToUpper());
    }
}

