using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Data.Services;
using SocialMediaApp.ViewModels.Friends;

namespace SocialMediaApp.ViewComponents;

public class SuggestedFriendsViewComponent :  ViewComponent
{
    private readonly IFriendsService _friendsService;
    public SuggestedFriendsViewComponent(IFriendsService friendsService)
    {
        _friendsService = friendsService;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var loggedInUserId = ((ClaimsPrincipal)User).FindFirstValue(ClaimTypes.NameIdentifier);
        var userId = int.Parse(loggedInUserId);
        var suggestedFriends = await _friendsService.GetSuggestedFriendsAsync(userId);
        var suggestedFriendsVM = suggestedFriends.Select(n => new UserWithFriendsCountVM()
        {
            UserId = n.User.Id,
            FullName = n.User.FullName,
            ProfilePictureUrl = n.User.ProfilePictureUrl,
            FriendsCount = n.FriendsCount,
        }).ToList();
        return View(suggestedFriendsVM);
    }
    
}