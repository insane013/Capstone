using Microsoft.EntityFrameworkCore;
using TodoListApp.Services.Database.Entities;

namespace TodoListApp.Services.Database;

/// <summary>
/// Database context for TodoLists.
/// </summary>
public class TodoListDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TodoListDbContext"/> class.
    /// </summary>
    /// <param name="options">Database options.</param>
    public TodoListDbContext(DbContextOptions<TodoListDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets DbSet with all TodoLists.
    /// </summary>
    public DbSet<TodoListEntity> TodoLists { get; set; }

    /// <summary>
    /// Gets or sets DbSet with all TodoTasks.
    /// </summary>
    public DbSet<TodoTaskEntity> TodoTasks { get; set; }

    /// <summary>
    /// Gets or sets DbSet with all TodoList Access info.
    /// </summary>
    public DbSet<UserTodoAccess> Accesses { get; set; }

    /// <summary>
    /// Gets or sets DbSet with all comments.
    /// </summary>
    public DbSet<CommentEntity> Comments { get; set; }

    /// <summary>
    /// Gets or sets DbSet with all Invites.
    /// </summary>
    public DbSet<InviteEntity> Invites { get; set; }

    /// <summary>
    /// Gets or sets DbSet with all Tags.
    /// </summary>
    public DbSet<TaskTagEntity> Tags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        _ = modelBuilder?.Entity<UserTodoAccess>().HasNoKey();

        _ = modelBuilder?.Entity<UserTodoAccess>()
            .HasKey(x => new { x.UserId, x.TodoListId });

        _ = modelBuilder?.Entity<UserTodoAccess>()
            .HasOne(x => x.TodoList)
            .WithMany(x => x.TodoAccesses)
            .HasForeignKey(x => x.TodoListId)
            .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder?.Entity<TodoListEntity>()
            .HasMany(x => x.TodoTasks)
            .WithOne(x => x.TodoList)
            .HasForeignKey(x => x.TodoListId)
            .OnDelete(DeleteBehavior.Cascade);

        _ = modelBuilder?.Entity<InviteEntity>(entity =>
            {
                _ = entity.HasOne(i => i.TodoList)
                    .WithMany(t => t.Invites)
                    .HasForeignKey(i => i.TodoListId)
                    .OnDelete(DeleteBehavior.Cascade);

                _ = entity.HasIndex(i => new { i.UserId, i.TodoListId })
                    .IsUnique()
                    .HasDatabaseName("IX_Invite_UserId_TodoListId");
            });

        _ = modelBuilder?.Entity<TaskTagEntity>(entity =>
        {
            _ = entity.HasOne(e => e.TodoList)
                .WithMany(e => e.Tags)
                .HasForeignKey(e => e.TodoListId)
                .OnDelete(DeleteBehavior.NoAction);

            _ = entity.HasIndex(e => new { e.TodoListId, e.Tag })
                .IsUnique();

            _ = entity.HasMany(e => e.RelatedTasks)
                .WithMany(t => t.Tags);
        });
    }
}
