using AutoMapper;
using TodoListApp.Models.Tag;
using TodoListApp.Services.Database.Entities;

namespace TodoListApp.Services.WebApi.Mapper;

public class TagMappingProfile : Profile
{
    public TagMappingProfile()
    {
        _ = this.CreateMap<TaskTagEntity, TagModel>()
            .ForMember(dest => dest.TodoListTitle, opt => opt.MapFrom(x => x.TodoList.Title));

        _ = this.CreateMap<TagModel, TaskTagEntity>();
    }
}
