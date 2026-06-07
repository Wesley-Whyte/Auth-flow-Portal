using Microsoft.AspNetCore.Mvc;
using authflow.Portal.Models;

namespace authflow.Portal.Controllers;

/// <summary>Handles account-related actions such as login and logout.</summary>
public class AccountController : Controller
{
    private readonly ILogger<AccountController> _logger;

    /// <summary>Initializes a new instance of <see cref="AccountController"/>.</summary>
    /// <param name="logger">The logger instance for this controller.</param>
    public AccountController(ILogger<AccountController> logger)
    {
        _logger = logger;
    }

    /// <summary>Renders the login form.</summary>
    /// <returns>The Login view with an empty <see cref="LoginViewModel"/>.</returns>
    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    /// <summary>Processes the submitted login form.</summary>
    /// <param name="model">The credentials submitted by the user.</param>
    /// <returns>
    /// Redirects to the home page on success; re-renders the login view with errors on failure.
    /// </returns>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // TODO: replace with real credential validation (ASP.NET Core Identity, OAuth, etc.)
        _logger.LogInformation("Login attempt for {Email}", model.Email);

        return RedirectToAction("Index", "Home");
    }
}
