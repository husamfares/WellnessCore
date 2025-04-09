using System;

namespace API.Dtos;

public class UserDto
{
    public required string Username { get; set; }
    public required string Token { get; set; }

    public required string Gender { get; set; }
    
    public int Id { get; set; }  // Include the user Id

}
