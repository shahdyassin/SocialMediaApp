using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaApp.Controllers.Base;
using SocialMediaApp.Data;
using SocialMediaApp.Data.Helpers.Enums;
using SocialMediaApp.Data.Models;
using SocialMediaApp.Data.Services;
using SocialMediaApp.ViewModels.Stories;

namespace SocialMediaApp.Controllers
{
    [Authorize]

    public class StoriesController : BaseController
    {
        private readonly IStoriesService _services;
        private readonly IFilesService _files;
        public StoriesController(IStoriesService services , IFilesService files)
        {
           _services = services;
              _files = files;
        }
        [HttpPost]
        public async Task<IActionResult> CreateStory(StoryVM story)
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null)
                return RedirectToLogIn();

            var imageUploadPath = await _files.UploadImageAsync(story.Image, ImageFileType.StoryImage);
            var newStory = new Story
            {
                DateCreated = DateTime.UtcNow,
                IsDeleted = false,
                ImageUrl = imageUploadPath,
                UserId = loggedInUserId.Value
            };
           
           await _services.CreateStoryAsync(newStory);
            return RedirectToAction("Index" , "Home");
        }
    }
}
