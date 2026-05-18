using AutoMapper;
using CareNota.DTOs.Admin;
using CareNota.Models;

namespace CareNota.Mappings;

public class AdminProfile : Profile
{
    public AdminProfile()
    {
        // ApplicationUser → AdminProfileDto
        CreateMap<ApplicationUser, AdminProfileDto>()
            .ForMember(Dest => Dest.Id,
                       Opt => Opt.MapFrom(Src => Src.Id))
            .ForMember(Dest => Dest.FullName,
                       Opt => Opt.MapFrom(Src => Src.FullName))
            .ForMember(Dest => Dest.Email,
                       Opt => Opt.MapFrom(Src => Src.Email))
            .ForMember(Dest => Dest.PhoneNumber,
                       Opt => Opt.MapFrom(Src => Src.PhoneNumber))
            .ForMember(Dest => Dest.Gender,
                       Opt => Opt.MapFrom(Src => Src.Gender))
            .ForMember(Dest => Dest.CreatedAt,
                       Opt => Opt.MapFrom(Src => Src.CreatedAt));

        // UpdateAdminProfileDto → ApplicationUser (reverse: ignore Id)
        CreateMap<UpdateAdminProfileDto, ApplicationUser>()
     .ForMember(dest => dest.FullName,
         opt => opt.MapFrom(src => src.FullName))
     .ForMember(dest => dest.PhoneNumber,
         opt => opt.MapFrom(src => src.PhoneNumber))
     .ForMember(dest => dest.Gender,
         opt => opt.MapFrom(src => src.Gender))

     // IMPORTANT: prevent overwriting these fields
     .ForMember(dest => dest.Id, opt => opt.Ignore())
     .ForMember(dest => dest.Email, opt => opt.Ignore())
     .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}