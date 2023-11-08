using AutoMapper;
using BlogNetworkB.Infrastructure.Mapping;
using ConnectionLib;
using ConnectionLib.DAL;
using Microsoft.EntityFrameworkCore;
using ConnectionLib.DAL.Repositories.Interfaces;
using ConnectionLib.DAL.Repositories;
using NLog;
using NLog.Web;
using BlogNetworkB.BLL.Services.Interfaces;
using BlogNetworkB.BLL.Services;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
try
{
    var builder = WebApplication.CreateBuilder(args);

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

    builder.Services.AddControllersWithViews();

    // DI for NLog
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

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
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}