namespace SocialMediaApp.ViewModels.Friends;

public class UserWithFriendsCountVM
{
    public int UserId { get; set; }
    public string FullName { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public int FriendsCount { get; set; }

    public string FriendsCountDisplay => FriendsCount == 0 ? "No Followers" :
        FriendsCount == 1 ? "1 Follower" : $"{FriendsCount} Follower";
}