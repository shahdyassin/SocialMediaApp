using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Data.Helpers.Enums;
using SocialMediaApp.Data.Services;
using SocialMediaApp.ViewModels.Settings;

namespace SocialMediaApp.Controllers
{
    [Authorize]

    public class SettingsController : Controller
    {
        private readonly IUsersService _service;
        private readonly IFilesService _file;
        public SettingsController(IUsersService service , IFilesService file)
        {
            _service = service;
            _file = file;
        }
        public async Task<IActionResult> Index()
        {
            var loggedInUserId = 1; 
            var userDb = await _service.GetUser(loggedInUserId); 
            return View(userDb);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(UpdateProfilePictureVM pictureVM)
        {
            var loggedInUserId = 1;
            var uploadedProfilePictureUrl = await _file.UploadImageAsync(pictureVM.ProfilePictureImage , ImageFileType.ProfilePicture);

            await _service.UpdateUserProfilePicture(loggedInUserId, uploadedProfilePictureUrl);

            return RedirectToAction("Index");

        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UpdateProfileVM profile)
        {
            return RedirectToAction("Index");

        }
        [HttpPost]
        public async Task<IActionResult> UpdatePassword(UpdatePasswordVM passwordVM)
        {
            return RedirectToAction("Index");

        }
    }
}
