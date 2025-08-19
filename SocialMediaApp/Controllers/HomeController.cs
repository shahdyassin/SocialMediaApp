using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaApp.Controllers.Base;
using SocialMediaApp.Data;
using SocialMediaApp.Data.Helpers;
using SocialMediaApp.Data.Helpers.Enums;
using SocialMediaApp.Data.Models;
using SocialMediaApp.Data.Services;
using SocialMediaApp.ViewModels.Home;

namespace SocialMediaApp.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IPostService _service;
        private readonly IHashtagsService _hashtags;
        private readonly IFilesService _files;

        public HomeController(ILogger<HomeController> logger ,IPostService service , IHashtagsService hashtags , IFilesService files)
        {
            _logger = logger;
            _hashtags = hashtags;
            _service = service;
            _files = files;
        }

        public async Task<IActionResult> Index()
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null)
                return RedirectToLogIn();
            var allPosts = await _service.GetAllPostsAsync(loggedInUserId.Value);
            return View(allPosts);
        }
        public async Task<IActionResult> Details(int postId)
        {
            var post = await _service.GetPostByIdAsync(postId);
            return View(post);
        }
        [HttpPost]
        public async Task<IActionResult> CreatePost(PostVM post)
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null)
                return RedirectToLogIn();

            var imageUploadPath = await _files.UploadImageAsync(post.Image, ImageFileType.PostImage);
            // This should be replaced with actual user ID from session or authentication context

            //Create Post
            var newPost = new Post
            {
                Content = post.Content,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                ImageUrl = imageUploadPath,
                NrOfReports = 0,
                UserId = loggedInUserId.Value
            };
            
            await _service.CreatePostAsync(newPost);
          await _hashtags.ProcessHashtagsForNewPostAsync(post.Content);


            //Check for Hashtags in the post content


            //Redirect to the index action to show the updated list of posts
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePostLike(PostLikeVM postLikeVM)
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null)
                return RedirectToLogIn();
            await _service.TogglePostLikeAsync(postLikeVM.PostId , loggedInUserId.Value);

           var post = await _service.GetPostByIdAsync(postLikeVM.PostId); 
            return PartialView("Home/_Post" , post);
        }
        [HttpPost]
        public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM favoriteVM)
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null)
                return RedirectToLogIn();
             
            await _service.TogglePostFavoriteAsync(favoriteVM.PostId, loggedInUserId.Value);
            
            var post = await _service.GetPostByIdAsync(favoriteVM.PostId); 
            return PartialView("Home/_Post" , post);
        }
        [HttpPost]
        public async Task<IActionResult> TogglePostVisibility(PostVisibilityVM visibilityVM)
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null)
                return RedirectToLogIn();

           await _service.TogglePostVisibilityAsync(visibilityVM.PostId, loggedInUserId.Value);
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPostComment(PostCommentVM commentVM)
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null)
                return RedirectToLogIn();

            
            //Create Comment
            var newComment = new Comment
            {
                Content = commentVM.Content,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                PostId = commentVM.PostId,
                UserId = loggedInUserId.Value
            };
          

            await _service.AddPostCommentAsync(newComment);
            var post = await _service.GetPostByIdAsync(commentVM.PostId);
            return PartialView("Home/_Post", post);
            //Redirect to the index action to show the updated list of posts
        }
        [HttpPost]
        public async Task<IActionResult> AddPostReport(PostReportVM reportVM)
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null)
                return RedirectToLogIn();

            await _service.ReportPostAsync(reportVM.PostId, loggedInUserId.Value);

            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePostComment(RemoveCommentVM commentVM)
        {
            await _service.RemovePostCommentAsync(commentVM.CommentId);
            var post = await _service.GetPostByIdAsync(commentVM.PostId);
            return PartialView("Home/_Post", post);
        }
        [HttpPost]
        public async Task<IActionResult> PostRemove(PostRemoveVM removeVM)
        {
            var postRemoved = await _service.RemovePostAsync(removeVM.PostId);
            await _hashtags.ProcessHashtagsForRemovePostAsync(postRemoved.Content);
            return RedirectToAction("Index");
        }
    }
}
