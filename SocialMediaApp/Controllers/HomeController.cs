using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaApp.Data;
using SocialMediaApp.Data.Helpers;
using SocialMediaApp.Data.Models;
using SocialMediaApp.ViewModels.Home;

namespace SocialMediaApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger , AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            int loggedInUserId = 1;
            var allPosts = await _context.Posts
                .Where(n => (!n.IsPrivate || n.UserId == loggedInUserId) && n.Reports.Count < 5 && !n.IsDeleted)
                .Include(n => n.User)
                .Include(n => n.Likes)
                .Include(n => n.Favorites)
                .Include(n => n.Comments).ThenInclude(n => n.User)
                .Include(n => n.Reports)
                .OrderByDescending(n => n.DateCreated)
                .ToListAsync();
            return View(allPosts);
        }
        [HttpPost]
        public async Task<IActionResult> CreatePost(PostVM post)
        {
            int loggedInUser = 1;
            // This should be replaced with actual user ID from session or authentication context

            //Create Post
            var newPost = new Post
            {
                Content = post.Content,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                ImageUrl = "",
                NrOfReports = 0,
                UserId = loggedInUser
            };
            //Check and Save the Image
            if(post.Image != null && post.Image.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (post.Image.ContentType.Contains("image"))
                {
                    string rootFolderPathImages = Path.Combine(rootFolderPath, "images/Posts");
                    Directory.CreateDirectory(rootFolderPathImages);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(post.Image.FileName);
                    string filePath = Path.Combine(rootFolderPathImages, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await post.Image.CopyToAsync(stream);
                    }

                    //Set URL to new Post Object
                    newPost.ImageUrl = "/images/Posts/" + fileName;
                }
            }

            //Add the new post to the database
            await _context.Posts.AddAsync(newPost);
            await _context.SaveChangesAsync();


            //Check for Hashtags in the post content
            var postHashtags = HashtagHelper.GetHashtags(post.Content);
            foreach(var hashtag in postHashtags)
            {
                var hashtagDb = await _context.Hashtags
                    .FirstOrDefaultAsync(h => h.Name == hashtag);
                if(hashtagDb != null)
                {
                    hashtagDb.Count++;
                    hashtagDb.DateUpdated = DateTime.UtcNow;

                    _context.Hashtags.Update(hashtagDb);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    var newHashtag = new Hashtag
                    {
                        Name = hashtag,
                        Count = 1,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow
                    };
                    await _context.Hashtags.AddAsync(newHashtag);
                    await _context.SaveChangesAsync(); 
                }
            }

            //Redirect to the index action to show the updated list of posts
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> TogglePostLike(PostLikeVM postLikeVM)
        {
            int loggedInUserId = 1;

            //Check if the user has already liked the post
            var like = await _context.Likes
                .Where(l => l.PostId == postLikeVM.PostId && l.UserId == loggedInUserId)
                .FirstOrDefaultAsync();

            if (like != null)
            {
                 _context.Likes.Remove(like);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newLike = new Like
                {
                    PostId = postLikeVM.PostId,
                    UserId = loggedInUserId
                };
                await _context.Likes.AddAsync(newLike);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> TogglePostFavorite(PostFavoriteVM favoriteVM)
        {
            int loggedInUserId = 1;

            //Check if the user has already favorited the post
            var fav = await _context.Favorites
                .Where(l => l.PostId == favoriteVM.PostId && l.UserId == loggedInUserId)
                .FirstOrDefaultAsync();

            if (fav != null)
            {
                _context.Favorites.Remove(fav);
                await _context.SaveChangesAsync();
            }
            else
            {
                var newFav = new Favorite
                {
                    PostId = favoriteVM.PostId,
                    UserId = loggedInUserId
                };
                await _context.Favorites.AddAsync(newFav);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> TogglePostVisibility(PostVisibilityVM visibilityVM)
        {
            int loggedInUserId = 1;

            //Get Post by Id and logged in User
            var post = await _context.Posts
                .FirstOrDefaultAsync(l => l.Id == visibilityVM.PostId && l.UserId == loggedInUserId);

            if (post != null)
            {
                post.IsPrivate = !post.IsPrivate;
                 _context.Posts.Update(post);
                await _context.SaveChangesAsync();
            }
           
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
            //Add the new comment to the database
            await _context.Comments.AddAsync(newComment);
            await _context.SaveChangesAsync();
            //Redirect to the index action to show the updated list of posts
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> AddPostReport(PostReportVM reportVM)
        {
            int loggedInUserId = 1;
            
            var newReport = new Report
            {
                DateCreated = DateTime.UtcNow,
                PostId = reportVM.PostId,
                UserId = loggedInUserId
            };
            
            await _context.Reports.AddAsync(newReport);
            await _context.SaveChangesAsync();
            //Redirect to the index action to show the updated list of posts
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> RemovePostComment(RemoveCommentVM commentVM)
        {
            var commentDb = await _context.Comments.FirstOrDefaultAsync(c => c.Id == commentVM.CommentId);

            if(commentDb != null)
            {
                _context.Comments.Remove(commentDb);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> PostRemove(PostRemoveVM removeVM)
        {
            var postDb = await _context.Posts
                .FirstOrDefaultAsync(p => p.Id == removeVM.PostId);

            if(postDb != null)
            {
                postDb.IsDeleted = true; // Soft delete
                _context.Posts.Update(postDb);
                await _context.SaveChangesAsync();

                //Update Hashtags
                var postHashtags = HashtagHelper.GetHashtags(postDb.Content);
                foreach (var hashtag in postHashtags)
                {
                    var hashtagDb = await _context.Hashtags
                        .FirstOrDefaultAsync(h => h.Name == hashtag);
                    if (hashtagDb != null)
                    {
                        hashtagDb.Count--;
                       hashtagDb.DateUpdated = DateTime.UtcNow;
                        
                        _context.Hashtags.Update(hashtagDb);
                        await _context.SaveChangesAsync();
                    }
                } 
            }

            

            return RedirectToAction("Index");
        }
    }
}
