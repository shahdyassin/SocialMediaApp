using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SocialMediaApp.Data;
using SocialMediaApp.Data.Services;

namespace SocialMediaApp.ViewComponents
{
    public class StoriesViewComponent : ViewComponent
    {
        private readonly IStoriesService _services;
        public StoriesViewComponent(IStoriesService services)
        {
            _services = services;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var allStories = await _services.GetAllStoriesAsync();

            return View(allStories);
        }
    }
}
