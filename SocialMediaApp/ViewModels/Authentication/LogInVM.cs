using System.ComponentModel.DataAnnotations;

namespace SocialMediaApp.ViewModels.Authentication;

public class LogInVM
{
    [Required(ErrorMessage = "Email Is Required")]
    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Password Is Required")]
    public string Password { get; set; }
}