using RaceDay.Web.Models;
using System.Net.Http.Json;

namespace RaceDay.Web.Services;

public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpContextAccessor = httpContextAccessor;
        AttachSessionCookie();
    }

    private void AttachSessionCookie()
    {
        var cookie = _httpContextAccessor.HttpContext?.Session.GetString("ApiSessionCookie");
        if (!string.IsNullOrEmpty(cookie))
        {
            _httpClient.DefaultRequestHeaders.Remove("Cookie");
            _httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
        }
    }

    // ---- AUTH ----

    public async Task<HttpResponseMessage> RegisterAsync(object registerDto)
    {
        return await _httpClient.PostAsJsonAsync("api/auth/register", registerDto);
    }

    public async Task<HttpResponseMessage> LoginAsync(object loginDto)
    {
        return await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);
    }

    // ---- Generic helpers ----

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        return await _httpClient.GetFromJsonAsync<T>(endpoint);
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
    {
        return await _httpClient.PostAsJsonAsync(endpoint, data);
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string endpoint, T data)
    {
        return await _httpClient.PutAsJsonAsync(endpoint, data);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
    {
        return await _httpClient.DeleteAsync(endpoint);
    }

// ---- EVENTS ----
public async Task<List<EventViewModel>?> GetEventsAsync() =>
    await GetAsync<List<EventViewModel>>("api/events");

    public async Task<EventViewModel?> GetEventAsync(int id) =>
        await GetAsync<EventViewModel>($"api/events/{id}");

    // ---- CATEGORIES ----
    public async Task<List<CategoryViewModel>?> GetCategoriesForEventAsync(int eventId) =>
        await GetAsync<List<CategoryViewModel>>($"api/events/{eventId}/categories");

    // ---- ENROLMENTS ----
    public async Task<HttpResponseMessage> EnrolAsync(EnrolRequestViewModel dto) =>
        await PostAsync("api/enrolments", dto);

    public async Task<List<EnrolmentViewModel>?> GetMyEnrolmentsAsync() =>
        await GetAsync<List<EnrolmentViewModel>>("api/enrolments/my");

    // ---- RESULTS ----
    public async Task<List<ResultViewModel>?> GetMyResultsAsync() =>
        await GetAsync<List<ResultViewModel>>("api/results/my");

    // ---- PROFILE ----
    public async Task<ProfileViewModel?> GetProfileAsync() =>
        await GetAsync<ProfileViewModel>("api/profile");

    public async Task<HttpResponseMessage> UpdateProfileAsync(ProfileViewModel dto) =>
        await PutAsync("api/profile", dto);

    // ---- EVENTS (Organiser) ----
    public async Task<HttpResponseMessage> CreateEventAsync(EventFormViewModel dto) =>
        await PostAsync("api/events", dto);

    public async Task<HttpResponseMessage> UpdateEventAsync(int id, EventFormViewModel dto) =>
        await PutAsync($"api/events/{id}", dto);

    public async Task<HttpResponseMessage> DeleteEventAsync(int id) =>
        await DeleteAsync($"api/events/{id}");

    // ---- CATEGORIES (Organiser) ----
    public async Task<HttpResponseMessage> CreateCategoryAsync(int eventId, CategoryFormViewModel dto) =>
        await PostAsync($"api/events/{eventId}/categories", dto);

    public async Task<HttpResponseMessage> UpdateCategoryAsync(int id, CategoryFormViewModel dto) =>
        await PutAsync($"api/categories/{id}", dto);

    public async Task<HttpResponseMessage> DeleteCategoryAsync(int id) =>
        await DeleteAsync($"api/categories/{id}");

    // ---- ENROLMENTS (Organiser) ----
    public async Task<List<EnrolmentViewModel>?> GetEventEnrolmentsAsync(int eventId) =>
  
       await GetAsync<List<EnrolmentViewModel>>($"api/events/{eventId}/enrolments");
    // ---- RESULTS (Organiser) ----
    public async Task<List<ResultViewModel>?> GetEventResultsAsync(int eventId) =>
        await GetAsync<List<ResultViewModel>>($"api/events/{eventId}/results");

    public async Task<HttpResponseMessage> CreateResultAsync(ResultFormViewModel dto) =>
        await PostAsync("api/results", dto);

    public async Task<HttpResponseMessage> UpdateResultAsync(int id, ResultFormViewModel dto) =>
        await PutAsync($"api/results/{id}", dto);
}