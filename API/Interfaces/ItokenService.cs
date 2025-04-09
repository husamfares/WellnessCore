using System;
using API.Entities;

namespace API.Interfaces;

public interface ItokenService
{
    Task<string> CreateToken(AppUser user);
}
