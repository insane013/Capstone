using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database.Entities;

namespace TodoListApp.Services.Database;

/// <summary>
/// Class containing methods for migration and population DB with standard data.
/// </summary>
public static class SeedData
{
    /// <summary>
    /// Populate database with standard data.
    /// </summary>
    /// <param name="context">Database context.</param>
    public static void Populate(TodoListDbContext context)
    {
        if (context == null)
        {
            return;
        }

        context.Database.Migrate();

        if (!context.TodoLists.Any())
        {
            _ = context.TodoLists.Add(new TodoListEntity
            {
                Id = 0,
                Title = "Learn Japanese",
                Description = "Tasks for a week for learning Japanese",
            });

            _ = context.TodoLists.Add(new TodoListEntity
            {
                Id = 0,
                Title = "Study ASP.Net",
                Description = "Tasks for a week for studying programming with ASP.Net Core",
            });

            _ = context.TodoLists.Add(new TodoListEntity
            {
                Id = 0,
                Title = "Listen to Ado!",
                Description = "Lsit of songs from Ado you MUST listen to!",
            });

            _ = context.SaveChanges();
        }
    }
}
