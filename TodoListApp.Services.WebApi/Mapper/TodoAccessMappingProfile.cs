using AutoMapper;
using TodoListApp.Models.TodoList;
using TodoListApp.Services.Database.Entities;

namespace TodoListApp.Services.WebApi.Mapper;

public class TodoAccessMappingProfile : Profile
{
    public TodoAccessMappingProfile()
    {
        _ = this.CreateMap<TodoAccessModel, UserTodoAccess>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(x => x.UserId))
            .ForMember(dest => dest.TodoListId, opt => opt.MapFrom(x => x.TodoId))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(x => x.Role));

        _ = this.CreateMap<UserTodoAccess, TodoAccessModel>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(x => x.UserId))
            .ForMember(dest => dest.TodoId, opt => opt.MapFrom(x => x.TodoListId))
            .ForMember(dest => dest.Role, opt => opt.MapFrom(x => x.Role));
    }
}
