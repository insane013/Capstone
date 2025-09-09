using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database.Users.Identity;

namespace TodoListApp.Services.Database.Users;
public class UserDbContext : IdentityDbContext<User, IdentityRole, string>
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        _ = builder?.Entity<User>()
            .HasIndex(u => u.UniqueTag)
            .IsUnique();
    }
}
