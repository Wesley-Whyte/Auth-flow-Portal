using Microsoft.AspNetCore.Mvc;
using authflow.Portal.Models;

namespace authflow.Portal.Controllers;

/// <summary>Handles account-related actions such as login and logout.</summary>
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>Initialises a new instance of <see cref="AccountController"/>.</summary>
    /// <param name="logger">The logger instance for this controller.</param>
    /// <param name="httpClientFactory">Factory used to create the auth API HTTP client.</param>
    public AccountController(ILogger<AccountController> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
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

        var client = _httpClientFactory.CreateClient("AuthApi");
        var payload = new { username = model.UsernameOrEmail, model.Password };
        var response = await client.PostAsJsonAsync("account/login", payload);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(model);
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
        if (result?.Token is null)
        {
            ModelState.AddModelError(string.Empty, "Authentication failed. Please try again.");
            return View(model);
        }

        // HttpOnly prevents JavaScript access; Secure ensures the cookie is only sent over HTTPS.
        Response.Cookies.Append("jwt", result.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddHours(1)
        });

        _logger.LogInformation("Login successful for {UsernameOrEmail}", model.UsernameOrEmail);
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

    /// <summary>Represents the JSON response body returned by the authentication API.</summary>
    /// <param name="Token">The JWT issued by the authentication API.</param>
    private sealed record AuthResponse
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
