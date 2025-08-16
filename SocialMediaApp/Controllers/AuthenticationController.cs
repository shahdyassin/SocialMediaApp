using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Data.Helpers.Constants;
using SocialMediaApp.Data.Models;
using SocialMediaApp.ViewModels.Authentication;
using SocialMediaApp.ViewModels.Settings;

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
        var existingUser = await _userManager.FindByEmailAsync(logIn.Email);
        if (existingUser == null)
        {
            ModelState.AddModelError("", "Invalid Email/Password. Please try again.");
            return View(logIn);
        }
        
        var existingUserClaims = await _userManager.GetClaimsAsync(existingUser);
        if(!existingUserClaims.Any(c => c.Type == CustomClaim.FullName))
            await _userManager.AddClaimAsync(existingUser, new Claim(CustomClaim.FullName, existingUser.FullName));
            
        var result = await _signInManager.PasswordSignInAsync(existingUser.UserName , logIn.Password, false,false);
        if (result.Succeeded)
        {
           
            return RedirectToAction("Index", "Home");
        }
            
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
        var existingUser = await _userManager.FindByEmailAsync(register.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError("Email", "Email already exists");
            return View(register);
        }
        var result = await _userManager.CreateAsync(newUser, register.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(newUser , AppRoles.User);
            await _userManager.AddClaimAsync(newUser , new Claim(CustomClaim.FullName, newUser.FullName));
            await _signInManager.SignInAsync(newUser, false);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
        return View(register);
    }
    [HttpPost]
    public async Task<IActionResult> UpdatePassword(UpdatePasswordVM passwordVM)
    {
        if (passwordVM.NewPassword != passwordVM.ConfirmPassword)
        {
            TempData["PasswordError"] = "Passwords do not match.";
            TempData["ActiveTab"] = "Password";
            return RedirectToAction("Index", "Settings");
        }
        var loggedInUser = await _userManager.GetUserAsync(User);
        var isCurrentPasswordValid = await _userManager.CheckPasswordAsync(loggedInUser, passwordVM.CurrentPassword);

        if (!isCurrentPasswordValid)
        {
            TempData["PasswordError"] = "Current Password is not Valid";
            TempData["ActiveTab"] = "Password";
            return RedirectToAction("Index", "Settings");
        }
        
        var result = await _userManager.ChangePasswordAsync(loggedInUser, passwordVM.CurrentPassword, passwordVM.NewPassword);

        if (result.Succeeded)
        {
            TempData["PasswordSuccess"] = "Password changed successfully";
            TempData["ActiveTab"] = "Password";

            await _signInManager.RefreshSignInAsync(loggedInUser);
            
        }
        
        return RedirectToAction("Index", "Settings");

    }
    [HttpPost]
    public async Task<IActionResult> UpdateProfile(UpdateProfileVM profile)
    {
        var loggedInUser = await _userManager.GetUserAsync(User);
        if(loggedInUser == null)
            return RedirectToAction("LogIn");
        loggedInUser.FullName = profile.FullName;
        loggedInUser.UserName = profile.UserName;
        loggedInUser.Bio = profile.Bio;
        
        var result = await _userManager.UpdateAsync(loggedInUser);
        if (!result.Succeeded)
        {
            TempData["UserProfileError"] = "User Profile Couldn't be Updated.";
            TempData["ActiveTab"] = "Profile";
        }
        await _signInManager.RefreshSignInAsync(loggedInUser);
        return RedirectToAction("Index", "Settings");

    }
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("LogIn");
    }
    
}