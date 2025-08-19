using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Controllers.Base;
using SocialMediaApp.Data.Helpers.Constants;
using SocialMediaApp.Data.Models;
using SocialMediaApp.Data.Services;
using SocialMediaApp.ViewModels.Friends;

namespace SocialMediaApp.Controllers;

public class FriendsController : BaseController
{
    private readonly IFriendsService _friendsService;

    public FriendsController(IFriendsService friendsService)
    {
        _friendsService = friendsService;
    }
    // GET
    public async Task<IActionResult> Index()
    {
        var userId = GetUserId();
        if (!userId.HasValue) RedirectToLogIn();

        var friendRequest = new FriendShipVM()
        {
            Friends = await _friendsService.GetFriendsAsync(userId.Value),
            FriendRequestsSent = await _friendsService.GetSentFriendRequestAsync(userId.Value),
            FriendRequestsReceived = await _friendsService.GetReceivedFriendRequestAsync(userId.Value)
        };
        return View(friendRequest);
    }

    [HttpPost]
    public async Task<IActionResult> SendFriendRequest(int receiverId)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            RedirectToLogIn();
        
        await _friendsService.SendRequestAsync(userId.Value, receiverId);
        return RedirectToAction("Index" , "Home");
    }
    [HttpPost]
    public async Task<IActionResult> UpdateFriendRequest(int requestId , string status)
    {
        await _friendsService.UpdateRequestAsync(requestId, status);
        return RedirectToAction("Index");
    } 
    [HttpPost]
    public async Task<IActionResult> RemoveFriend(int friendshipId)
    {
        await _friendsService.RemoveFriendAsync(friendshipId);
        return RedirectToAction("Index");
    }
}