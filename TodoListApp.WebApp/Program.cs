using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoListApp.Services.WebApp.Helpers;
using TodoListApp.Services.WebApp.Interfaces;
using TodoListApp.Services.WebApp.Services;
using TodoListApp.WebApp.Helpers;

var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration["Jwt:Key"];
var issuer = builder.Configuration["Jwt:Issuer"];
var audience = builder.Configuration["Jwt:Audience"];

JwtTokenOptions.SetOptions(key!, issuer!, audience!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";  // Страница логина
    options.LogoutPath = "/Account/Logout";  // Страница логаута
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = JwtTokenOptions.GetValidationParameters();
});

// Add authorization
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
});

builder.Services.AddAntiforgery();

builder.Services.AddHttpClient<ITodoListWebApiService, TodoListWebApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Api:Uri"] ?? "/");
});

builder.Services.AddHttpClient<ITodoTaskWebApiService, TodoTaskWebApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Api:Uri"] ?? "/");
});

builder.Services.AddHttpClient<IAuthorizationWebApiService, AuthorizationWebApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Api:Uri"] ?? "/");
});

builder.Services.AddHttpClient<IUserWebApiService, UserWebApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Api:Uri"] ?? "/");
});

builder.Services.AddHttpClient<ICommentWebApiService, CommentWebApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Api:Uri"] ?? "/");
});

builder.Services.AddHttpClient<IInviteWebApiService, InviteWebApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Api:Uri"] ?? "/");
});

builder.Services.AddHttpClient<ITagWebApiService, TagWebApiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Api:Uri"] ?? "/");
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IFilterStorageService, FilterStorageService>();
builder.Services.AddScoped<IFilterHandler, FilterHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    _ = app.UseExceptionHandler("/Home/Error");
    _ = app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapDefaultControllerRoute();

await app.RunAsync();
