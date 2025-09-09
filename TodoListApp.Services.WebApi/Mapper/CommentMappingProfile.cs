using AutoMapper;
using TodoListApp.Models.Comments;
using TodoListApp.Models.Comments.DTO;
using TodoListApp.Services.Database.Entities;

namespace TodoListApp.Services.WebApi.Mapper;

public class CommentMappingProfile : Profile
{
    public CommentMappingProfile()
    {
        _ = this.CreateMap<CommentEntity, CommentModel>()
            .ForMember(dest => dest.TaskId, opt => opt.MapFrom(x => x.TodoTaskId));

        _ = this.CreateMap<CommentModel, CommentEntity>()
            .ForMember(dest => dest.TodoTaskId, opt => opt.MapFrom(x => x.TaskId));

        _ = this.CreateMap<CommentCreateDto, CommentEntity>()
            .ForMember(dest => dest.TodoTaskId, opt => opt.MapFrom(x => x.TaskId))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedUserId, opt => opt.Ignore());

        _ = this.CreateMap<CommentUpdateDto, CommentEntity>()
            .ForMember(dest => dest.TodoTaskId, opt => opt.MapFrom(x => x.TaskId))
            .ForMember(dest => dest.CreatedTime, opt => opt.Ignore());

        _ = this.CreateMap<CommentDeleteDto, CommentEntity>()
            .ForMember(dest => dest.TodoTaskId, opt => opt.MapFrom(x => x.TaskId))
            .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
            .ForMember(dest => dest.Content, opt => opt.Ignore());
    }
}
