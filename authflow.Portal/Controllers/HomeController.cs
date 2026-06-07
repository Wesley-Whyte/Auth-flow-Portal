using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using authflow.Portal.Models;

namespace authflow.Portal.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    /// <summary>Renders the user profile page with stubbed profile data.</summary>
    /// <returns>The Profile view populated with a <see cref="Models.UserProfileViewModel"/>.</returns>
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
