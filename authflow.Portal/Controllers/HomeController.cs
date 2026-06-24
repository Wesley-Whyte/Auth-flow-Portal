using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using authflow.Portal.Models;
using Microsoft.AspNetCore.Authorization;

namespace authflow.Portal.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;

    /// <summary>Initialises the controller with logging and configuration dependencies.</summary>
    /// <param name="logger">Logger for this controller.</param>
    /// <param name="configuration">Application configuration, populated from environment variables and appsettings.</param>
    public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    /// <summary>Renders the home page with current environment configuration values.</summary>
    /// <returns>The Index view populated with an <see cref="EnvironmentConfigViewModel"/>.</returns>
    public IActionResult Index()
    {
        var model = new EnvironmentConfigViewModel(
            Environment:           System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            AuthApiBaseUrl:        _configuration["AuthApi:BaseUrl"]                           ?? "(not set)",
            JwtIssuer:             _configuration["JWT:Issuer"]                                ?? "(not set)",
            JwtSigninKey:          _configuration["JWT:SigninKey"]                             ?? "(not set)",
            GoogleClientId:        _configuration["Authentication:Google:ClientId"]            ?? "(not set)",
            GoogleClientSecret:    _configuration["Authentication:Google:ClientSecret"]        ?? "(not set)",
            MicrosoftClientId:     _configuration["Authentication:Microsoft:ClientId"]         ?? "(not set)",
            MicrosoftClientSecret: _configuration["Authentication:Microsoft:ClientSecret"]     ?? "(not set)"
        );
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>Renders the user profile page with stubbed profile data.</summary>
    /// <returns>The Profile view populated with a <see cref="Models.UserProfileViewModel"/>.</returns>
    [Authorize]
    public IActionResult Profile()
    {
        // Stub data — replace with identity/database lookup once auth is wired up.
        var model = new Models.UserProfileViewModel
        {
            FullName = "Wesley Ofori",
            Username = "wesof",
            Email = "wesoflife@gmail.com",
            Bio = "Software developer building authflow.Portal. Passionate about clean auth flows and great UX.",
            Role = "Admin",
            JoinedDate = new DateTime(2025, 1, 15),
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
