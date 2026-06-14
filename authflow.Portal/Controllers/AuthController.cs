using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using authflow.Portal.Models;
using authflow.Application.Interfaces;

namespace authflow.Portal.Controllers;

/// <summary>Handles auth-related actions such as login and logout.</summary>
public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;
    private readonly IAuthService _authService;

    /// <summary>Initialises a new instance of <see cref="AuthController"/>.</summary>
    /// <param name="logger">The logger instance for this controller.</param>
    /// <param name="authService">The auth service used to perform login and registration.</param>
    public AuthController(ILogger<AuthController> logger, IAuthService authService)
    {
        _logger = logger;
        _authService = authService;
    }

    /// <summary>Renders the login form.</summary>
    /// <returns>The Login view with an empty <see cref="LoginViewModel"/>.</returns>
    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    /// <summary>Submits credentials to the authentication API and, on success, stores the
    /// returned JWT in an HttpOnly cookie before redirecting the user.</summary>
    /// <param name="model">The credentials submitted by the user.</param>
    /// <param name="returnUrl">The URL to redirect to after a successful login.</param>
    /// <returns>
    /// Redirects to <paramref name="returnUrl"/> (or the home page) on success;
    /// re-renders the login view with a model error on failure.
    /// </returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var result = await _authService.LoginAsync(model.UsernameOrEmail, model.Password);

            // HttpOnly prevents JavaScript access; Secure ensures the cookie is only sent over HTTPS.
            Response.Cookies.Append("jwt", result, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            _logger.LogInformation("Login successful for {UsernameOrEmail}", model.UsernameOrEmail);
            return LocalRedirect(returnUrl ?? Url.Action("Index", "Home")!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login failed for {UsernameOrEmail}", model.UsernameOrEmail);
            ModelState.AddModelError(string.Empty, "An error occurred during login. Please try again.");
            return View(model);
        }
    }

    /// <summary>Initiates an external OAuth challenge for the specified provider.</summary>
    /// <param name="provider">The authentication provider name (e.g. "Google", "Microsoft").</param>
    /// <param name="returnUrl">The URL to redirect to after a successful login.</param>
    /// <returns>A challenge result that redirects the browser to the provider's consent screen.</returns>
    [HttpGet]
    public IActionResult ExternalLogin(string provider, string? returnUrl = null)
    {
        var redirectUrl = Url.Action("ExternalLoginCallback", "Auth", new { returnUrl });
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        return Challenge(properties, provider);
    }

    /// <summary>Handles the OAuth callback, signs the user in via the Cookie scheme, and redirects.</summary>
    /// <param name="returnUrl">The URL to redirect to after a successful login.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// Redirects to <paramref name="returnUrl"/> (or the home page) on success;
    /// redirects to the login page if the external authentication failed.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null)
    {
        var result = await HttpContext.AuthenticateAsync("ExternalCookie");
        if (!result.Succeeded)
            return RedirectToAction("Login");

        var email = result.Principal!.FindFirstValue(ClaimTypes.Email) ?? "";
        var name = result.Principal.FindFirstValue(ClaimTypes.Name) ?? email;
        var provider = result.Properties?.Items[".AuthScheme"] ?? "External";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, email),
            new(ClaimTypes.Name, name),
            new(ClaimTypes.Email, email),
            new("provider", provider)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        await HttpContext.SignOutAsync("ExternalCookie");

        _logger.LogInformation("External login successful via {Provider} for {Email}", provider, email);
        return LocalRedirect(returnUrl ?? Url.Action("Index", "Home")!);
    }

    /// <summary>Clears the authentication cookie and redirects to the login page.</summary>
    /// <returns>A redirect to the Login action.</returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt");
        return RedirectToAction("Login");
    }

    /// <summary>Renders the registration form.</summary>
    /// <returns>The Register view with an empty <see cref="RegisterViewModel"/>.</returns>
    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    /// <summary>Submits registration data to the authentication API and, on success, redirects
    /// to the login page with a confirmation message.</summary>
    /// <param name="model">The registration details submitted by the user.</param>
    /// <returns>
    /// Redirects to <see cref="Login()"/> on success;
    /// re-renders the register view with a model error on failure.
    /// </returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var response = await _authService.RegisterAsync(model.Username, model.Email, model.Password);

            // HttpOnly prevents JavaScript access; Secure ensures the cookie is only sent over HTTPS.
            Response.Cookies.Append("jwt", response, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            _logger.LogInformation("Login successful for {Username}", model.Username);
            return LocalRedirect(Url.Action("Index", "Home")!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Registration failed.");
            ModelState.AddModelError(string.Empty, "An error occurred during registration.");
            return View(model);
        }
    }
}
