using AutoMapper;
using TodoListApp.Models.User;
using TodoListApp.Services.Database.Users.Identity;

namespace TodoListApp.Services.WebApi.Mapper;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        _ = this.CreateMap<User, ViewUserInfo>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(x => x.Email))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(x => x.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(x => x.Name))
            .ForMember(dest => dest.RegistrationTime, opt => opt.MapFrom(x => x.RegistrationTime))
            .ForMember(dest => dest.UniqueTag, opt => opt.MapFrom(x => x.UniqueTag));
    }
}
