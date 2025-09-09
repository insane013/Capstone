using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TodoListApp.Services;
using TodoListApp.Services.Database;
using TodoListApp.Services.Database.Interfaces;
using TodoListApp.Services.Database.Repositories;
using TodoListApp.Services.Database.Users;
using TodoListApp.Services.Database.Users.Identity;
using TodoListApp.Services.WebApi.Mapper;
using TodoListApp.Services.WebApi.TodoList;
using TodoListApp.Services.WebApi.Users;
using TodoListApp.WebApi.Helpers;
using TodoListApp.WebApi.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(1);
    options.IncludeSubDomains = true;
});

builder.Services.AddDbContext<TodoListDbContext>(options =>
{
    _ = options.UseSqlServer(builder.Configuration["ConnectionStrings:TodoListDbConnectionString"]);
});

// Authentication setup
builder.Services.AddDbContext<UserDbContext>(options =>
        options.UseSqlServer(builder.Configuration["ConnectionStrings:UserDbConnectionString"]));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<UserDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,

            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(builder.Configuration["Jwt:Key"]),
            ValidateIssuerSigningKey = true,
        };
    });

// Services
builder.Services.AddScoped<ITodoListDatabaseService, TodoListDatabaseService>();
builder.Services.AddScoped<ITodoListRepository, TodoListRepository>();

builder.Services.AddScoped<ITodoTaskDatabaseService, TodoTaskDatabaseService>();
builder.Services.AddScoped<ITodoTaskRepository, TodoTaskRepository>();

builder.Services.AddScoped<ITodoAccessDatabaseService, TodoAccessDatabaseService>();
builder.Services.AddScoped<ITodoAccessRepository, TodoAccessRepository>();

builder.Services.AddScoped<IUserDatabaseService, UserDatabaseService>();

builder.Services.AddScoped<ICommentDatabaseService, CommentDatabaseService>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

builder.Services.AddScoped<IInviteDatabaseService, InviteDatabaseService>();
builder.Services.AddScoped<IInviteRepository, InviteRepository>();

builder.Services.AddScoped<ITagDatabaseService, TagDatabaseService>();
builder.Services.AddScoped<ITagRepository, TagRepository>();

// Auto-Mappers
builder.Services.AddAutoMapper(typeof(TaskMappingProfile));
builder.Services.AddAutoMapper(typeof(TodoListMappingProfile));
builder.Services.AddAutoMapper(typeof(UserMappingProfile));
builder.Services.AddAutoMapper(typeof(TodoAccessMappingProfile));
builder.Services.AddAutoMapper(typeof(CommentMappingProfile));
builder.Services.AddAutoMapper(typeof(InviteMappingProfile));
builder.Services.AddAutoMapper(typeof(TagMappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    _ = app.UseHsts();
}

app.UseHttpsRedirection();

app.UseMiddleware<ApiExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
