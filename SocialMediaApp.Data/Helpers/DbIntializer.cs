using Microsoft.AspNetCore.Identity;
using SocialMediaApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocialMediaApp.Data.Helpers.Constants;

namespace SocialMediaApp.Data.Helpers
{
    public static class DbIntializer
    {
        public static async Task SeedUsersAndRolesAsync(UserManager<User> userManager , RoleManager<IdentityRole<int>> roleManager)
        {
            //Roles
            if (!roleManager.Roles.Any())
            {
                foreach (var roleName in AppRoles.All )
                {
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                    }
                }
            }
            //Users with Roles
            if (!userManager.Users.Any(n => !string.IsNullOrEmpty(n.Email)))
            {
                var userPassword = "Shahd@123*";
                var newUser = new User
                {
                      UserName = "shahdyassin",
                      Email = "shahdyassin63@gmail.com",
                      FullName = "Shahd Yassin",
                      ProfilePictureUrl = "https://img-b.udemycdn.com/user/200_H/16004620_10db_5.jpg",
                      EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(newUser, userPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(newUser, AppRoles.User); 
                }
                
                
                var newAdmin = new User
                {
                    UserName = "admin.admin",
                    Email = "testadmin@gmail.com",
                    FullName = "Admin Shahd",
                    ProfilePictureUrl = "https://img-b.udemycdn.com/user/200_H/16004620_10db_5.jpg",
                    EmailConfirmed = true
                };
                var resultnewAdmin = await userManager.CreateAsync(newAdmin, userPassword);
                if (resultnewAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, AppRoles.Admin); 
                }
            }
            
        }
        public static async Task SeedAsync(AppDbContext appDbContext)
        {
            if(!appDbContext.Users.Any() && !appDbContext.Posts.Any())
            {
                var newUser = new User()
                {
                    FullName = "Shahd Yassin",
                    ProfilePictureUrl = "https://img-b.udemycdn.com/user/200_H/16004620_10db_5.jpg"
                };
                await appDbContext.Users.AddAsync(newUser);
                await appDbContext.SaveChangesAsync();

                var newPageWithoutImage = new Post()
                {
                    Content = "This is going to be our first post which is being loaded from the database and it has been created using our test user.",
                    ImageUrl = "",
                    NrOfReports = 0,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,

                    UserId = newUser.Id
                };
                var newPostWithImage = new Post()
                {
                    Content = "This is going to be our first post which is being loaded from the database and it has been created using our test user. This post has an image",
                    ImageUrl = "https://unsplash.com/photos/foggy-mountain-summit-1Z2niiBPg5A",
                    NrOfReports = 0,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,

                    UserId = newUser.Id
                };

                await appDbContext.Posts.AddRangeAsync(newPageWithoutImage, newPostWithImage);
                await appDbContext.SaveChangesAsync();
            }
            
        }
    }
}
