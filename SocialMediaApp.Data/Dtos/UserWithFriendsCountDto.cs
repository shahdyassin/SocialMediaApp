using SocialMediaApp.Data.Models;

namespace SocialMediaApp.Data.Dtos;

public class UserWithFriendsCountDto
{
    public User User { get; set; }
    public int FriendsCount { get; set; }
}