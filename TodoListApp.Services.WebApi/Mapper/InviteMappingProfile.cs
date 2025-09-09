using AutoMapper;
using TodoListApp.Models.Invite;
using TodoListApp.Models.Invite.DTO;
using TodoListApp.Services.Database.Entities;

namespace TodoListApp.Services.WebApi.Mapper;

public class InviteMappingProfile : Profile
{
    public InviteMappingProfile()
    {
        _ = this.CreateMap<InviteEntity, InviteModel>()
            .ForMember(dest => dest.TodoListTitle, opt => opt.MapFrom(x => x.TodoList.Title));

        _ = this.CreateMap<InviteCreateDto, InviteEntity>()
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        this.CreateMap<InviteCreateDto, List<InviteEntity>>()
            .ConvertUsing((src, dest, context) =>
                src.Users.Select(userId =>
                {
                    var entity = context.Mapper.Map<InviteEntity>(src);
                    entity.UserId = userId;
                    return entity;
                }).ToList());

        _ = this.CreateMap<InviteUpdateDto, InviteEntity>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore());

        _ = this.CreateMap<InviteDeleteDto, InviteEntity>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Message, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore());

        _ = this.CreateMap<InviteResponseDto, InviteEntity>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Message, opt => opt.Ignore());
    }
}
