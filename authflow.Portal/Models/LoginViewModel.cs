using System.ComponentModel.DataAnnotations;

namespace authflow.Portal.Models;

/// <summary>Represents the credentials submitted on the login form.</summary>
public class LoginViewModel
{
    /// <summary>Gets or sets the username or email address used to identify the account.</summary>
    [Required(ErrorMessage = "Username or email is required.")]
    [Display(Name = "username")]
    public string UsernameOrEmail { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's password.</summary>
    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "password")]
    public string Password { get; set; } = string.Empty;

    /// <summary>Gets or sets a value indicating whether a persistent auth cookie should be issued.</summary>
    [Display(Name = "Remember me")]
    public bool RememberMe { get; set; }
}
