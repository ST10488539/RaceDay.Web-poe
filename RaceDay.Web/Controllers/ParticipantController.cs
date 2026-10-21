using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers;

public class ParticipantController : Controller
{
    private readonly ApiClient _apiClient;

    public ParticipantController(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    private bool IsLoggedInParticipant()
    {
        var role = HttpContext.Session.GetString("UserRole");
        return role == "Participant";
    }

    // GET /Participant  -> events home with filtering
    public async Task<IActionResult> Index(string? type, string? search)
    {
        if (!IsLoggedInParticipant())
            return RedirectToAction("Login", "Account");

        var events = await _apiClient.GetEventsAsync() ?? new List<EventViewModel>();

        if (!string.IsNullOrWhiteSpace(type))
            events = events.Where(e => e.EventType.Equals(type, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrWhiteSpace(search))
            events = events.Where(e =>
                e.EventName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                e.Location.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

        ViewBag.SelectedType = type;
        ViewBag.Search = search;

        return View(events);
    }

    // GET /Participant/Details/5 -> event detail + categories
    public async Task<IActionResult> Details(int id)
    {
        if (!IsLoggedInParticipant())
            return RedirectToAction("Login", "Account");

        var ev = await _apiClient.GetEventAsync(id);
        if (ev == null) return NotFound();

        var categories = await _apiClient.GetCategoriesForEventAsync(id) ?? new List<CategoryViewModel>();

        ViewBag.Categories = categories;
        return View(ev);
    }

    // POST /Participant/Enrol
    [HttpPost]
    public async Task<IActionResult> Enrol(int eventId, int categoryId)
    {
        if (!IsLoggedInParticipant())
            return RedirectToAction("Login", "Account");

        var response = await _apiClient.EnrolAsync(new EnrolRequestViewModel { CategoryID = categoryId });

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "You have been enrolled successfully.";
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            TempData["Error"] = $"Enrolment failed: {error}";
        }

        return RedirectToAction("Details", new { id = eventId });
    }

    // GET /Participant/MyEnrolments
    public async Task<IActionResult> MyEnrolments()
    {
        if (!IsLoggedInParticipant())
            return RedirectToAction("Login", "Account");

        var enrolments = await _apiClient.GetMyEnrolmentsAsync() ?? new List<EnrolmentViewModel>();
        return View(enrolments);
    }

    // GET /Participant/MyResults
    public async Task<IActionResult> MyResults()
    {
        if (!IsLoggedInParticipant())
            return RedirectToAction("Login", "Account");

        var results = await _apiClient.GetMyResultsAsync() ?? new List<ResultViewModel>();
        return View(results);
    }

    // GET /Participant/Profile
    public async Task<IActionResult> Profile()
    {
        if (!IsLoggedInParticipant())
            return RedirectToAction("Login", "Account");

        var profile = await _apiClient.GetProfileAsync();
        if (profile == null) return NotFound();

        return View(profile);
    }

    // POST /Participant/Profile
    [HttpPost]
    public async Task<IActionResult> Profile(ProfileViewModel model)
    {
        if (!IsLoggedInParticipant())
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid) return View(model);

        var response = await _apiClient.UpdateProfileAsync(model);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction("Profile");
        }

        var error = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", $"Update failed: {error}");
        return View(model);
    }
}