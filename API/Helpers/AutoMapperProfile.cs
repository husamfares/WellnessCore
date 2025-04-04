using API.Dtos;
using API.Entities;
using AutoMapper;

namespace API.Helpers;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        // Mapping for RegisterDto (not really used, but keeping it)
        // CreateMap<RegisterDto, AppUser>()
        //     .ForMember(dest => dest.UserName, opt => opt.Ignore()) 
        //     .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) 
        //     .ForMember(dest => dest.PasswordSalt, opt => opt.Ignore());

        // Mapping for WellnessInfoDto → AppUser
        CreateMap<WellnessInfoDto, AppUser>()
            .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.Age))
            .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
            .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.Height))
            .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender));

    }
}
