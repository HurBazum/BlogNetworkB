using AutoMapper;
using BlogNetworkB.Infrastructure.Mapping;
using ConnectionLib;
using BlogNetworkB.DAL;
using Microsoft.EntityFrameworkCore;
using BlogNetworkB.DAL.Repositories.Interfaces;
using BlogNetworkB.DAL.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Create mapper
var configMap = new MapperConfiguration(c => c.AddProfile(new MappingProfile()));
IMapper mapper = configMap.CreateMapper();

// Add services to the container.
builder.Services.AddDbContext<BlogContext>(options => options.UseSqlite(SQLiteBaseBuilder.GetConnectionString(AppDomain.CurrentDomain)))
                .AddScoped<IAuthorRepository, AuthorRepository>()
                .AddScoped<IRoleRepository, RoleRepository>()
                .AddScoped<IArticleRepository, ArticleRepository>()
                .AddScoped<ICommentRepository, CommentRepository>()
                .AddScoped<ITagRepository, TagRepository>()
                .AddSingleton(mapper);

// Add Cookies
builder.Services.AddAuthentication(options => options.DefaultScheme = "Cookies")
                .AddCookie("Cookies", options =>
                {
                    options.Events = new Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = redirectContext =>
                        {
                            redirectContext.HttpContext.Response.StatusCode = 401;
                            return Task.CompletedTask;
                        }
                    };
                });

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();