using NEXT_BMS.Models;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using NEXT_BMS.Utilities;
using Insya.Localization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using NEXT_BMS.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace NEXT_BMS;

public class Startup
{
    public static class Settings
    {
        public static IConfiguration Configuration;
    }
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        Settings.Configuration = configuration;
    }
    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDistributedMemoryCache();
        services.AddHttpContextAccessor();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        services.AddDbContext<NEXT_BMSContext>(options =>
        options.UseSqlServer(Configuration.GetConnectionString("NEXT_BMSConnection"))
        );
        services.Configure<MailSettings>(Configuration.GetSection("MailSettings"));
        services.Configure<AppConfig>(Configuration.GetSection("Appconfig"));
        services.AddTransient<IMailService, MailService>();
        services.AddTransient<IValidateRequest, ValidateRequestConcrete>();
        services.AddScoped<IExcelImporter, ExcelImporter>();
        services.AddScoped<IExcelExport, ExcelExport>();
        services.AddScoped<Localization, Localization>();
        services.AddScoped<UserManagement, UserManagement>();
        services.AddScoped<CurrentUser, CurrentUser>();
        services.AddScoped<Configurations, Configurations>();
        //services.AddScoped<DocumentManagement, DocumentManagement>();
        services.AddControllers(config =>
        {
            config.Filters.Add(new AccessFilters());
        });
        services.AddControllersWithViews();
        services.AddControllersWithViews(config => config.Filters.Add(typeof(CustomExceptionFilter)));
        services.AddDistributedMemoryCache();
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.Use((context, next) =>
            {
                context.Request.Scheme = "https";
                return next();
            });
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseSession();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapAreaControllerRoute(
                name: "Administrator",
                areaName: "Administrator",
                pattern: "Administrator/{controller=Home}/{action=Index}/{id?}");
            endpoints.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        });
    }
}