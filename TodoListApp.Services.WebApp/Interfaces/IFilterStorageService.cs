using TodoListApp.Models;

namespace TodoListApp.Services.WebApp.Interfaces;
public interface IFilterStorageService
{
    public void SaveFilter<T>(string name, T filter) where T : BaseFilter;
    public T? LoadFilter<T>(string name) where T : BaseFilter;
}
