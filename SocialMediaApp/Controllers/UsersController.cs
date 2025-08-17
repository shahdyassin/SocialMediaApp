using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Controllers.Base;
using SocialMediaApp.Data.Models;
using SocialMediaApp.Data.Services;
using SocialMediaApp.ViewModels.Users;

namespace SocialMediaApp.Controllers;

public class UsersController : BaseController
{
    private readonly IUsersService _usersService;
    private readonly UserManager<User>  _userManager;
    public UsersController(IUsersService usersService , UserManager<User> userManager)
    {
        _usersService = usersService;
        _userManager = userManager;
    }
    // GET
    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Details(int userId)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        var userPosts = await _usersService.GetUserPosts(userId);
        var userprofileVM = new GetUserProfileVM()
        {
            User = user,
          Posts = userPosts
        };
        return View(userprofileVM);
    }
}