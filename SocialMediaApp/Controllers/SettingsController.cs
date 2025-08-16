using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Controllers.Base;
using SocialMediaApp.Data.Helpers.Enums;
using SocialMediaApp.Data.Models;
using SocialMediaApp.Data.Services;
using SocialMediaApp.ViewModels.Settings;

namespace SocialMediaApp.Controllers
{
    [Authorize]

    public class SettingsController : BaseController
    {
        private readonly IUsersService _service;
        private readonly IFilesService _file;
        private readonly UserManager<User> _userManager;
        public SettingsController(IUsersService service , IFilesService file , UserManager<User> userManager)
        {
            _service = service;
            _file = file;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
           
            
            var loggedInUser = await _userManager.GetUserAsync(User);
            return View(loggedInUser);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(UpdateProfilePictureVM pictureVM)
        {
            var loggedInUserId = GetUserId();
            if (loggedInUserId == null)
                return RedirectToLogIn();
            var uploadedProfilePictureUrl = await _file.UploadImageAsync(pictureVM.ProfilePictureImage , ImageFileType.ProfilePicture);

            await _service.UpdateUserProfilePicture(loggedInUserId.Value, uploadedProfilePictureUrl);

            return RedirectToAction("Index");

        }

    }
}
