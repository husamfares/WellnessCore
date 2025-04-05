using API.Dtos;
using API.Entities;
using API.Extensions;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
       CreateMap<AppUser, MemberDto>()
    .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>
        src.DateOfBirth.HasValue ? src.DateOfBirth.Value.CalculateAge() : 0));


    }
}
