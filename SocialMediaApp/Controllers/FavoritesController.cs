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
            int loggedInUserId = 1;
            var myFavoritePosts = await _postService.GetAllFavoritedPostsAsync(loggedInUserId); 
            return View(myFavoritePosts);
        }
    }
}
