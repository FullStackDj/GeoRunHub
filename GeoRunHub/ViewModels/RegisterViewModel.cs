using System.ComponentModel.DataAnnotations;

namespace GeoRunHub.ViewModels;

public class RegisterViewModel
{
    [Display(Name = "Email address")]
    [Required(ErrorMessage = "Enter your email address")]
    public string EmailAddress { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Display(Name = "Confirm password")]
    [Required(ErrorMessage = "Confirm your password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}