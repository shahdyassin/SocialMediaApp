using SocialMediaApp.Data.Models;

namespace SocialMediaApp.ViewModels.Users;

public class GetUserProfileVM
{
    public User User { get; set; }
    public List<Post> Posts { get; set; }
    
}