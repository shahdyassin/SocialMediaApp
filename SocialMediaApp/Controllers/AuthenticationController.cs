using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Data.Helpers.Constants;
using SocialMediaApp.Data.Models;
using SocialMediaApp.ViewModels.Authentication;

namespace SocialMediaApp.Controllers;

public class AuthenticationController : Controller
{
    private readonly UserManager<User>  _userManager;
    private readonly SignInManager<User> _signInManager;
    public AuthenticationController(UserManager<User>  userManager, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }
    // GET
    public async Task<IActionResult> LogIn()
    {
        return View();
    } 
    [HttpPost]
    public async Task<IActionResult> LogIn(LogInVM logIn)
    {
        if(!ModelState.IsValid)
            return View(logIn);
        
        var result = await _signInManager.PasswordSignInAsync(logIn.Email , logIn.Password, false,false);
        if(result.Succeeded)
            return RedirectToAction("Index", "Home");
        ModelState.AddModelError("", "Invalid login attempt.");
        return View(logIn);
            
    } 
    public async Task<IActionResult> Register()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Register(RegisterVM register)
    {
        if (!ModelState.IsValid)
            return View(register);
        
        var newUser = new User()
        {
          FullName = $"{register.FirstName} {register.LastName}",
          Email = register.Email,
          UserName = register.Email
        };
        var existingUser = await _userManager.FindByEmailAsync(newUser.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError("Email", "Email already exists");
            return View(register);
        }
        var result = await _userManager.CreateAsync(newUser, register.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(newUser , AppRoles.User);
            await _signInManager.SignInAsync(newUser, false);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
        return View(register);
    }
    
}