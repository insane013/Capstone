using AutoMapper;
using TodoListApp.Models.TodoList;
using TodoListApp.Models.TodoList.DTO;
using TodoListApp.Services.Database.Entities;

namespace TodoListApp.Services.WebApi.Mapper;
public class TodoListMappingProfile : Profile
{
    public TodoListMappingProfile()
    {
        _ = this.CreateMap<TodoListEntity, TodoListModel>()
            .ForMember(dest => dest.TodoListId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Description) ? null : src.Description))
            .ForMember(dest => dest.Tasks, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentUserAccessInfo, opt => opt.Ignore())
            .ForMember(dest => dest.TaskCount, opt => opt.MapFrom(src => src.TodoTasks.Count))
            .ForMember(dest => dest.CompletedTaskCount, opt => opt.MapFrom(src =>
                src.TodoTasks.Count(x => x.IsCompleted)));

        _ = this.CreateMap<TodoListModel, TodoListEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TodoListId))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty))
            .ForMember(dest => dest.TodoTasks, opt => opt.Ignore())
            .ForMember(dest => dest.TodoAccesses, opt => opt.Ignore());

        _ = this.CreateMap<TodoListCreateDto, TodoListEntity>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty))
            .ForMember(dest => dest.CreatedTime, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
            .ForMember(dest => dest.TodoTasks, opt => opt.Ignore())
            .ForMember(dest => dest.TodoAccesses, opt => opt.Ignore());

        _ = this.CreateMap<TodoListUpdateDto, TodoListEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.TodoListId))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty))
            .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())
            .ForMember(dest => dest.OwnerId, opt => opt.Ignore())
            .ForMember(dest => dest.TodoTasks, opt => opt.Ignore())
            .ForMember(dest => dest.TodoAccesses, opt => opt.Ignore());
    }
}
