
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using WebApplication1.Models;
using WebApplication1.Services;
using WebApplication1.Services.Interfaces;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSingleton<IUrlService,UrlService>();
            builder.Services.AddSingleton<IUserService, UserService>();

            builder.Services.AddDbContext<UrlContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("UrlConnection")),ServiceLifetime.Singleton);
            builder.Services.AddCors(options =>

            {
                options.AddPolicy("CorsPolicy",
                    policy => policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
            /*builder.Services.AddAuthentication().AddGoogle(GoogleOptions =>
            {
                GoogleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                GoogleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                GoogleOptions.CallbackPath = "/signin-google";
            });*/
            builder.Services.AddAuthentication("CookieAuth").AddCookie("CookieAuth", options =>
            {
                options.Cookie.Name = "CookieAuth";
                options.LoginPath = "/user/Login";
                options.ExpireTimeSpan = TimeSpan.FromDays(99999);
            });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("MustBeARegisteredUser", policy =>
                {
                    policy.RequireClaim("UserRegisterStatus", "Registered");
                }
                );
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                //app.UseMigrationsEndPoint();
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors("CorsPolicy");
            //app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            /*app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");*/

            app.MapControllers();
            app.Run();
        }
    }
}