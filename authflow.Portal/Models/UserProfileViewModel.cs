namespace authflow.Portal.Models;

/// <summary>Represents the data displayed on the user profile page.</summary>
public class UserProfileViewModel
{
    /// <summary>Gets or sets the user's display name.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Gets or sets the username (login handle).</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>Gets or sets the user's email address.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Gets or sets a short bio or description written by the user.</summary>
    public string? Bio { get; set; }

    /// <summary>Gets or sets the user's role, e.g. Admin or User.</summary>
    public string Role { get; set; } = "User";

    /// <summary>Gets or sets the date the account was created.</summary>
    public DateTime JoinedDate { get; set; }

    /// <summary>Gets the initials derived from <see cref="FullName"/> for the avatar placeholder.</summary>
    public string Initials
    {
        get
        {
            var parts = FullName.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return parts.Length >= 2
                ? $"{parts[0][0]}{parts[^1][0]}"   // first + last initial
                : FullName.Length > 0 ? FullName[..1] : "?";
        }
    }
}