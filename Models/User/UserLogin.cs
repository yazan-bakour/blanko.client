using System.ComponentModel.DataAnnotations;

namespace Banko.Client.Models.User
{
  public class UserLogin
  {
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(5, ErrorMessage = "Password must be at least 5 characters")]
    public string Password { get; set; } = string.Empty;
  }
}