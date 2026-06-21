using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace authflow.Portal.Controllers;

/// <summary>Handles Entra External ID CIAM sign-in and sign-out flows via OIDC redirect.</summary>
public class EntraAuthController : Controller
{
    private readonly ILogger<EntraAuthController> _logger;

    /// <summary>Initialises a new instance of <see cref="EntraAuthController"/>.</summary>
    /// <param name="logger">The logger instance for this controller.</param>
    public EntraAuthController(ILogger<EntraAuthController> logger)
    {
        _logger = logger;
    }

    /// <summary>Initiates the Entra External ID OIDC sign-in challenge, redirecting the user
    /// to the hosted Entra login page.</summary>
    /// <param name="returnUrl">The local URL to redirect to after a successful sign-in.</param>
    /// <returns>An OIDC challenge result, or a redirect if the user is already authenticated.</returns>
    [AllowAnonymous]
    public IActionResult SignIn(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return LocalRedirect(returnUrl ?? Url.Action("Index", "Home")!);

        _logger.LogInformation("Initiating Entra External ID sign-in challenge");

        var properties = new AuthenticationProperties
        {
            RedirectUri = returnUrl ?? Url.Action("Index", "Home")
        };

        return Challenge(properties, "EntraOidc");
    }

    /// <summary>Signs the user out of the local Entra cookie and initiates a federated
    /// sign-out with Entra External ID, clearing the session on the identity provider.</summary>
    /// <returns>A sign-out result that clears the local cookie and redirects to the Entra
    /// end-session endpoint.</returns>
    [HttpGet]
    public async Task<IActionResult> EntraSignOut()
    {
        _logger.LogInformation("Signing out user {Name} from Entra External ID", User.Identity?.Name);

        // Clear the local Entra cookie first; the SignOut call below handles the
        // federated logout redirect to the Entra end-session endpoint.
        await HttpContext.SignOutAsync("EntraCookie");

        return SignOut(
            new AuthenticationProperties { RedirectUri = Url.Action("Index", "Home") },
            "EntraOidc");
    }
}
