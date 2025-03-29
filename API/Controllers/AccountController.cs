using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.Dtos;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(DataContext context, ItokenService tokenService) : BaseApiController
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if(await UserExists(registerDto.Username))
        {
            return BadRequest("Username is taken");
        }

        if(registerDto.Username == null || registerDto.Password ==null || registerDto.Username == "" || registerDto.Password =="" )
        {
            return BadRequest("username or password cant be empty");
        }

        using var hmac = new HMACSHA512();

        var user = new AppUser
        {
            UserName = registerDto.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
            PasswordSalt = hmac.Key
        };

        context.Add(user);
        await context.SaveChangesAsync();

        return new UserDto{
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };
    }
    // Login

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto logingDto)
    {
        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == logingDto.Username.ToLower());

        if(user == null) return Unauthorized("Invalid username");

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(logingDto.Password));

        for(int i=0; i<computedHash.Length; i++)
        {
            if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
        }

        return new UserDto
        {
            Username = user.UserName,
            Token = tokenService.CreateToken(user)
        };
    }



    private async Task<bool> UserExists(string username)
    {
        return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
    }
}
