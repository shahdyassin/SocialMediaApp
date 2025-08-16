using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Data.Services;

namespace SocialMediaApp.Controllers
{
    [Authorize]

    public class FavoritesController : Controller
    {
        private readonly IPostService _postService;
        public FavoritesController(IPostService postService)
        {
            _postService = postService;
        }
        public async Task<IActionResult> Index()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var myFavoritePosts = await _postService.GetAllFavoritedPostsAsync(int.Parse(loggedInUserId)); 
            return View(myFavoritePosts);
        }
    }
}
