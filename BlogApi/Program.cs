using AutoMapper;
using BlogNetworkB.Infrastructure.Mapping;
using BlogNetworkB.BLL.Services.Interfaces;
using BlogNetworkB.BLL.Services;
using ConnectionLib.DAL.Repositories.Interfaces;
using ConnectionLib.DAL.Repositories;
using ConnectionLib.DAL;
using ConnectionLib;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Create mapper
var configMap = new MapperConfiguration(c => c.AddProfile(new MappingProfile()));
IMapper mapper = configMap.CreateMapper();

// Add context, repos, services to the container.
builder.Services.AddDbContext<BlogContext>(options => options.UseSqlite(SQLiteBaseBuilder.GetConnectionString(AppDomain.CurrentDomain)))
                .AddScoped<IAuthorRepository, AuthorRepository>()
                .AddScoped<IRoleRepository, RoleRepository>()
                .AddScoped<IArticleRepository, ArticleRepository>()
                .AddScoped<ICommentRepository, CommentRepository>()
                .AddScoped<ITagRepository, TagRepository>()
                .AddScoped<IAuthorService, AuthorService>()
                .AddScoped<IRoleService, RoleService>()
                .AddScoped<IArticleService, ArticleService>()
                .AddScoped<ICommentService, CommentService>()
                .AddScoped<ITagService, TagService>()
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();