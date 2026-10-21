using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers;

public class OrganiserController : Controller
{
    private readonly ApiClient _apiClient;

    public OrganiserController(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    private bool IsLoggedInOrganiser()
    {
        var role = HttpContext.Session.GetString("UserRole");
        return role == "Organiser";
    }

    private int CurrentUserId => HttpContext.Session.GetInt32("UserId") ?? 0;

    // GET /Organiser/Dashboard
    public async Task<IActionResult> Dashboard()
    {
        if (!IsLoggedInOrganiser())
            return RedirectToAction("Login", "Account");

        var allEvents = await _apiClient.GetEventsAsync() ?? new List<EventViewModel>();
        var myEvents = allEvents.Where(e => e.OrganiserID == CurrentUserId).ToList();

        var dashboard = new OrganiserDashboardViewModel { Events = myEvents };

        foreach (var ev in myEvents)
        {
            var enrolments = await _apiClient.GetEventEnrolmentsAsync(ev.EventID) ?? new List<EnrolmentViewModel>();
            dashboard.EnrolmentCountsByEvent[ev.EventID] = enrolments.Count;
        }

        return View(dashboard);
    }

    // GET /Organiser/Events
    public async Task<IActionResult> Events()
    {
        if (!IsLoggedInOrganiser())
            return RedirectToAction("Login", "Account");

        var allEvents = await _apiClient.GetEventsAsync() ?? new List<EventViewModel>();
        var myEvents = allEvents.Where(e => e.OrganiserID == CurrentUserId).ToList();

        return View(myEvents);
    }

    // GET /Organiser/CreateEvent
    public IActionResult CreateEvent()
    {
        if (!IsLoggedInOrganiser())
            return RedirectToAction("Login", "Account");

        return View(new EventFormViewModel { EventDate = DateTime.Today.AddDays(30) });
    }

    // POST /Organiser/CreateEvent
    [HttpPost]
    public async Task<IActionResult> CreateEvent(EventFormViewModel model)
    {
        if (!IsLoggedInOrganiser())
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid) return View(model);

        var response = await _apiClient.CreateEventAsync(model);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Event created successfully.";
            return RedirectToAction("Events");
        }

        var error = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", $"Failed to create event: {error}");
        return View(model);
    }

    // GET /Organiser/EditEvent/5
    public async Task<IActionResult> EditEvent(int id)
    {
        if (!IsLoggedInOrganiser())
            return RedirectToAction("Login", "Account");

        var ev = await _apiClient.GetEventAsync(id);
        if (ev == null) return NotFound();

        var model = new EventFormViewModel
        {
            EventID = ev.EventID,
            EventName = ev.EventName,
            EventDate = ev.EventDate,
            Location = ev.Location,
            Description = ev.Description,
            Distance = ev.Distance,
            EventType = ev.EventType,
            ImageURL = ev.ImageURL
        };

        return View(model);
    }

    // POST /Organiser/EditEvent/5
    [HttpPost]
    public async Task<IActionResult> EditEvent(int id, EventFormViewModel model)
    {
        if (!IsLoggedInOrganiser())
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid) return View(model);

        var response = await _apiClient.UpdateEventAsync(id, model);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Event updated successfully.";
            return RedirectToAction("Events");
        }

        var error = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", $"Failed to update event: {error}");
        return View(model);
    }

    // POST /Organiser/DeleteEvent/5
    [HttpPost]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        if (!IsLoggedInOrganiser())
            return RedirectToAction("Login", "Account");

        var response = await _apiClient.DeleteEventAsync(id);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Event deleted successfully.";
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            TempData["Error"] = $"Failed to delete event: {error}";
        }

        return RedirectToAction("Events");
    }
    // GET /Organiser/Categories?eventId=5
    public async Task<IActionResult> Categories(int eventId)
    {
        if (!IsLoggedInOrganiser())
            return RedirectToAction("Login", "Account");

        var ev = await _apiClient.GetEventAsync(eventId);
        if (ev == null) return NotFound();

        var categories = await _apiClient.GetCategoriesForEventAsync(eventId) ?? new List<CategoryViewModel>();

        ViewBag.EventName = ev.EventName;
        ViewBag.EventId = eventId;

        return View(categories);
    }

    // GET /Organiser/CreateCategory?eventId=5
    public IActionResult CreateCategory(int eventId)
    {
        if (!IsLoggedInOrganiser())
            return RedirectToAction("Login", "Account");

        return View(new CategoryFormViewModel { EventID = eventId, MinAge = 0 });
    }

    // POST /Organiser/CreateCategory
    [HttpPost]
    public async Task<IActionResult> CreateCategory(CategoryFormViewModel model)
    {
        if (!IsLoggedInOrganiser())
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid) return View(model);

        var response = await _apiClient.CreateCategoryAsync(model.EventID, model);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Category added successfully.";
            return RedirectToAction("Categories", new { eventId = model.EventID });
        }

        var error = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", $"Failed to add category: {error}");
        return View(model);
    }

    // GET /Organiser/EditCategory/5
    public async Task<IActionResult> EditCategory(int id, int eventId)
    {
        if (!IsLoggedInOrganiser())
            return RedirectToAction("Login", "Account");

        var categories = await _apiClient.GetCategoriesForEventAsync(eventId) ?? new List<CategoryViewModel>();
        var category = categories.FirstOrDefault(c => c.CategoryID == id);
        if (category == null) return NotFound();

        var model = new CategoryFormViewModel
        {
            CategoryID = category.CategoryID,
            EventID = category.EventID,
            CategoryName = category.CategoryName,
            Distance = category.Distance,
            MinAge = category.MinAge,
            MaxAge = category.MaxAge,
            Description = category.Description
        };

        return View(model);
    }

    // POST /Organiser/EditCategory/5
    [HttpPost]
    public async Task<IActionResult> EditCategory(int id, CategoryFormViewModel model)
    {
        if (!IsLoggedInOrganiser())
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid) return View(model);

        var response = await _apiClient.UpdateCategoryAsync(id, model);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Category updated successfully.";
            return RedirectToAction("Categories", new { eventId = model.EventID });
        }

        var error = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", $"Failed to update category: {error}");
        return View(model);
    }

    // POST /Organiser/DeleteCategory/5
    [HttpPost]
    public async Task<IActionResult> DeleteCategory(int id, int eventId)
    {
        if (!IsLoggedInOrganiser())
            return RedirectToAction("Login", "Account");

        var response = await _apiClient.DeleteCategoryAsync(id);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Category deleted successfully.";
        }
        else
        {
            var error = await response.Content.ReadAsStringAsync();
            TempData["Error"] = $"Failed to delete category: {error}";
        }

        return RedirectToAction("Categories", new { eventId });
    }
    public async Task<IActionResult> Enrolments(int eventId)
    {
        if (!IsLoggedInOrganiser())
            return RedirectToAction("Login", "Account");

        var ev = await _apiClient.GetEventAsync(eventId);
        if (ev == null) return NotFound();

        var enrolments = await _apiClient.GetEventEnrolmentsAsync(eventId) ?? new List<EnrolmentViewModel>();

        ViewBag.EventName = ev.EventName;
        ViewBag.EventId = eventId;

        return View(enrolments);
    }
    // GET /Organiser/Results?eventId=5
    public async Task<IActionResult> Results(int eventId)
    {
        if (!IsLoggedInOrganiser())
            return RedirectToAction("Login", "Account");

        var ev = await _apiClient.GetEventAsync(eventId);
        if (ev == null) return NotFound();

        var enrolments = await _apiClient.GetEventEnrolmentsAsync(eventId) ?? new List<EnrolmentViewModel>();
        var results = await _apiClient.GetEventResultsAsync(eventId) ?? new List<ResultViewModel>();

        var model = new EventResultsViewModel
        {
            EventID = eventId,
            EventName = ev.EventName,
            Rows = enrolments.Select(e => new ResultRowViewModel
            {
                Enrolment = e,
                Result = results.FirstOrDefault(r => r.RegistrationID == e.RegistrationID)
            }).ToList()
        };

        return View(model);
    }

    // GET /Organiser/EnterResult?registrationId=3&eventId=5
    public async Task<IActionResult> EnterResult(int registrationId, int eventId)
    {
        if (!IsLoggedInOrganiser())
            return RedirectToAction("Login", "Account");

        var enrolments = await _apiClient.GetEventEnrolmentsAsync(eventId) ?? new List<EnrolmentViewModel>();
        var enrolment = enrolments.FirstOrDefault(e => e.RegistrationID == registrationId);
        if (enrolment == null) return NotFound();

        return View(new ResultFormViewModel
        {
            RegistrationID = registrationId,
            EventID = eventId,
            ParticipantName = enrolment.ParticipantName
        });
    }

    // POST /Organiser/EnterResult
    [HttpPost]
    public async Task<IActionResult> EnterResult(ResultFormViewModel model)
    {
        if (!IsLoggedInOrganiser())
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid) return View(model);

        var response = await _apiClient.CreateResultAsync(model);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Result saved.";
            return RedirectToAction("Results", new { eventId = model.EventID });
        }

        var error = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", $"Failed to save result: {error}");
        return View(model);
    }

    // GET /Organiser/EditResult?id=7&eventId=5
    public async Task<IActionResult> EditResult(int id, int eventId)
    {
        if (!IsLoggedInOrganiser())
            return RedirectToAction("Login", "Account");

        var results = await _apiClient.GetEventResultsAsync(eventId) ?? new List<ResultViewModel>();
        var r = results.FirstOrDefault(x => x.ResultID == id);
        if (r == null) return NotFound();

        return View(new ResultFormViewModel
        {
            ResultID = r.ResultID,
            RegistrationID = r.RegistrationID,
            EventID = eventId,
            ParticipantName = r.ParticipantName,
            FinishTime = r.FinishTime,
            ChipTime = r.ChipTime,
            OverallPosition = r.OverallPosition,
            CategoryPosition = r.CategoryPosition,
            ResultStatus = r.ResultStatus
        });
    }

    // POST /Organiser/EditResult
    [HttpPost]
    public async Task<IActionResult> EditResult(ResultFormViewModel model)
    {
        if (!IsLoggedInOrganiser())
            return RedirectToAction("Login", "Account");

        if (!ModelState.IsValid) return View(model);

        var response = await _apiClient.UpdateResultAsync(model.ResultID, model);

        if (response.IsSuccessStatusCode)
        {
            TempData["Success"] = "Result updated.";
            return RedirectToAction("Results", new { eventId = model.EventID });
        }

        var error = await response.Content.ReadAsStringAsync();
        ModelState.AddModelError("", $"Failed to update result: {error}");
        return View(model);
    }
}