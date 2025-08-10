using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaApp.Data;
using SocialMediaApp.Data.Helpers;
using SocialMediaApp.Data.Helpers.Enums;
using SocialMediaApp.Data.Models;
using SocialMediaApp.Data.Services;
using SocialMediaApp.ViewModels.Home;

namespace SocialMediaApp.Controllers
{
    public class HomeController : Controller
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
            int loggedInUserId = 1;
            var allPosts = await _service.GetAllPostsAsync(loggedInUserId);
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
            int loggedInUser = 1;

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
                UserId = loggedInUser
            };
            
            await _service.CreatePostAsync(newPost);
          await _hashtags.ProcessHashtagsForNewPostAsync(post.Content);


            //Check for Hashtags in the post content


            //Redirect to the index action to show the updated list of posts
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> TogglePostLike(PostLikeVM postLikeVM)
        {
            int loggedInUserId = 1;
            await _service.TogglePostLikeAsync(postLikeVM.PostId , loggedInUserId);

           
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM favoriteVM)
        {
            int loggedInUserId = 1;
             
            await _service.TogglePostFavoriteAsync(favoriteVM.PostId, loggedInUserId);

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> TogglePostVisibility(PostVisibilityVM visibilityVM)
        {
            int loggedInUserId = 1;

           await _service.TogglePostVisibilityAsync(visibilityVM.PostId, loggedInUserId);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> AddPostComment(PostCommentVM commentVM)
        {
            int loggedInUserId = 1;

            
            //Create Comment
            var newComment = new Comment
            {
                Content = commentVM.Content,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                PostId = commentVM.PostId,
                UserId = loggedInUserId
            };
          

            await _service.AddPostCommentAsync(newComment);
            //Redirect to the index action to show the updated list of posts
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> AddPostReport(PostReportVM reportVM)
        {
            int loggedInUserId = 1;

            await _service.ReportPostAsync(reportVM.PostId, loggedInUserId);

            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> RemovePostComment(RemoveCommentVM commentVM)
        {
           

            await _service.RemovePostCommentAsync(commentVM.CommentId);
            return RedirectToAction("Index");
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
