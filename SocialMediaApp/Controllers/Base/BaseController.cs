using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace SocialMediaApp.Controllers.Base;

public abstract class BaseController : Controller
{
    protected int? GetUserId()
    {
        var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(loggedInUserId))
            return null;
        return int.Parse(loggedInUserId);
    }

    protected IActionResult RedirectToLogIn()
    {
        return RedirectToAction("LogIn", "Authentication");
    }
}