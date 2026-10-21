using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers;

public class AccountController : Controller
{
    private readonly ApiClient _apiClient;

    public AccountController(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var response = await _apiClient.RegisterAsync(model);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Registration successful. Please log in.";
            return RedirectToAction("Login");
        }

        var errorContent = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", $"Registration failed: {errorContent}");
        return View(model);
    }

    [HttpGet]
    public IActionResult Login() => View(new LoginViewModel());

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var response = await _apiClient.LoginAsync(model);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(model);
        }

        // Capture the API's session cookie so we can forward it on future calls
        if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            var sessionCookie = cookies.FirstOrDefault();
            if (sessionCookie != null)
            {
                HttpContext.Session.SetString("ApiSessionCookie", sessionCookie);
            }
        }

        var result = await response.Content.ReadFromJsonAsync<LoginResult>();
        if (result != null)
        {
            HttpContext.Session.SetString("UserRole", result.Role);
            HttpContext.Session.SetString("UserName", result.Name);
            HttpContext.Session.SetInt32("UserId", result.UserId);
        }

        return result?.Role switch
        {
            "Organiser" => RedirectToAction("Dashboard", "Organiser"),
            "Participant" => RedirectToAction("Index", "Participant"),
            _ => RedirectToAction("Index", "Home")
        };
    }

    [HttpPost]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}

public class LoginResult
{
    public string Message { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}