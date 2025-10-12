using ApplicationLayer.Common.Extensions;
using ApplicationLayer.DTOs.Identity;
using ApplicationLayer.DTOs.User;
using AutoMapper;
using DomainLayer.Entities;

namespace ApplicationLayer.Mapping.UserAccounts;

public class UserAccountProfile : Profile
{
    public UserAccountProfile()
    {
        CreateMap<SignUpDto, UserAccount>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => PhoneNumberHelper.NormalizePhoneNumber(src.PhonePrefix, src.PhoneNumber)));

        CreateMap<SignUpDto, UserProfile>();

        CreateMap<UpdateUserProfileDto, UserProfile>()
        .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
        .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
        .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
        .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address));
    }
}