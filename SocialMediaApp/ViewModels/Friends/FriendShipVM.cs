using SocialMediaApp.Data.Models;

namespace SocialMediaApp.ViewModels.Friends;

public class FriendShipVM
{
    public List<Friendship> Friends = new List<Friendship>();
    public List<FriendRequest> FriendRequestsSent = new List<FriendRequest>();
    public List<FriendRequest> FriendRequestsReceived = new List<FriendRequest>();
    
}