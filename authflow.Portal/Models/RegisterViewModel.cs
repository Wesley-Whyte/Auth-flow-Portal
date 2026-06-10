using System.ComponentModel.DataAnnotations;

namespace authflow.Portal.Models;

/// <summary>Represents the data submitted on the create-account form.</summary>
public class RegisterViewModel
{
    /// <summary>Gets or sets the desired username for the new account.</summary>
    [Required(ErrorMessage = "Username is required.")]
    [Display(Name = "username")]
    public string Username { get; set; } = string.Empty;

    /// <summary>Gets or sets the email address for the new account.</summary>
    [Required(ErrorMessage = "Email address is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [Display(Name = "email address")]
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets the password chosen for the new account.</summary>
    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    [Display(Name = "password")]
    public string Password { get; set; } = string.Empty;

    /// <summary>Gets or sets the confirmation password; must match <see cref="Password"/>.</summary>
    [Required(ErrorMessage = "Please confirm your password.")]
    [DataType(DataType.Password)]
    [Display(Name = "confirm password")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
