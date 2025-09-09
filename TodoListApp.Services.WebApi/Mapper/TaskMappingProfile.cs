using AutoMapper;
using TodoListApp.Models.TodoTask;
using TodoListApp.Models.TodoTask.DTO;
using TodoListApp.Services.Database.Entities;

namespace TodoListApp.Services.WebApi.Mapper;
public class TaskMappingProfile : Profile
{
    public TaskMappingProfile()
    {
        _ = this.CreateMap<TaskUpdateDto, TodoTaskEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(x => x.TodoId))
            .ForMember(dest => dest.Description, opt => opt.NullSubstitute(string.Empty))
            .ForMember(dest => dest.UpdatedTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
            .ForMember(dest => dest.AssignedUserId, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore());

        _ = this.CreateMap<TaskCreateDto, TodoTaskEntity>()
            .ForMember(dest => dest.Description, opt => opt.NullSubstitute(string.Empty))
            .ForMember(dest => dest.UpdatedTime, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.AssignedUserId, opt => opt.Ignore())
            .ForMember(dest => dest.Tags, opt => opt.Ignore());

        _ = this.CreateMap<TodoTaskEntity, TodoTaskModel>()
            .ForMember(dest => dest.TodoId, opt => opt.MapFrom(x => x.Id))
            .ForMember(dest => dest.AssignedUserId, opt => opt.MapFrom(src => src.AssignedUserId))
            .ForMember(dest => dest.Priority, opt => opt.MapFrom(src => src.Priority >= 3 ? 3 : src.Priority))
            .ForMember(dest => dest.Tags, opt => opt.Ignore())
            .ForMember(dest => dest.AvailableTags, opt => opt.Ignore());

        _ = this.CreateMap<TaskUpdateDto, TaskUpdateTagsDto>();
    }
}
