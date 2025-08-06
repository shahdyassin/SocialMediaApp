using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaApp.Data;
using SocialMediaApp.Data.Models;
using SocialMediaApp.ViewModels.Stories;

namespace SocialMediaApp.Controllers
{
    public class StoriesController : Controller
    {
        private readonly AppDbContext _context;
        public StoriesController(AppDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> CreateStory(StoryVM story)
        {
            int loggedInUserId = 1;
            var newStory = new Story
            {
                DateCreated = DateTime.UtcNow,
                IsDeleted = false,
                UserId = loggedInUserId
            };
            if (story.Image != null && story.Image.Length > 0)
            {
                string rootFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                if (story.Image.ContentType.Contains("image"))
                {
                    string rootFolderPathImages = Path.Combine(rootFolderPath, "images/Stories");
                    Directory.CreateDirectory(rootFolderPathImages);

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(story.Image.FileName);
                    string filePath = Path.Combine(rootFolderPathImages, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await story.Image.CopyToAsync(stream);
                    }

                    //Set URL to new Post Object
                    newStory.ImageUrl = "/images/Stories/" + fileName;
                }
            }
            await _context.Stories.AddAsync(newStory);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index" , "Home");
        }
    }
}
