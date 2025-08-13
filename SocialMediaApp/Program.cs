using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialMediaApp.Data;
using SocialMediaApp.Data.Helpers;
using SocialMediaApp.Data.Models;
using SocialMediaApp.Data.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace SocialMediaApp
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSwaggerGen();
            //DataBase Configuration
            var dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(dbConnectionString));

            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<IHashtagsService, HashtagsService>();
            builder.Services.AddScoped<IStoriesService, StoriesService>();
            builder.Services.AddScoped<IFilesService, FilesService>();
            builder.Services.AddScoped<IUsersService, UsersService>();


            //identity Configuration
            builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
                {
                    //Password Settings 
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 4;
                })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Authentication/Login";
                options.AccessDeniedPath = "/Authentication/AccessDenied";
            });
            
            // builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            //     .AddCookie(options =>
            //     {
            //         options.LoginPath = "/Authentication/Login";
            //         options.AccessDeniedPath = "/Authentication/AccessDenied";
            //     });
            builder.Services.AddAuthorization();
            var app = builder.Build();

            //Seed Database with Initial Data
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await dbContext.Database.MigrateAsync();
                await DbIntializer.SeedAsync(dbContext);
                
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
                await DbIntializer.SeedUsersAndRolesAsync(userManager, roleManager);
            }

                // Configure the HTTP request pipeline.
                if (!app.Environment.IsDevelopment())
                {
                    app.UseExceptionHandler("/Home/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.4
                    app.UseSwagger();
                    app.UseSwaggerUI();
                    app.UseHsts();
                }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
