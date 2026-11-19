using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using RaceDay.MVC.Models;


namespace RaceDay.MVC.Services
{
    public class RaceDayApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public RaceDayApiService(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        private void AttachApiSessionCookie(HttpRequestMessage request)
        {
            var apiCookie = _httpContextAccessor.HttpContext?.Session.GetString("ApiSessionCookie");
            if (!string.IsNullOrEmpty(apiCookie))
            {
                request.Headers.Add("Cookie", apiCookie);
            }
        }

        // --- AUTHENTICATION ---
        public async Task<(bool Success, string Error)> RegisterAsync(RegisterViewModel model)
        {
            var payload = JsonSerializer.Serialize(model);
            var response = await _httpClient.PostAsync("api/auth/register",
                new StringContent(payload, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode) return (true, string.Empty);
            var err = await response.Content.ReadAsStringAsync();
            return (false, err);
        }

        public async Task<UserSessionDto?> LoginAsync(LoginViewModel model)
        {
            var payload = JsonSerializer.Serialize(model);
            var response = await _httpClient.PostAsync("api/auth/login",
                new StringContent(payload, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode) return null;

            // Capture the API's session cookie and store it in MVC session
            if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
            {
                var sessionCookie = string.Join("; ", cookies.Select(c => c.Split(';')[0]));
                _httpContextAccessor.HttpContext?.Session.SetString("ApiSessionCookie", sessionCookie);
            }

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserSessionDto>(json, _jsonOptions);
        }

        public async Task LogoutAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/auth/logout");
            AttachApiSessionCookie(request);
            await _httpClient.SendAsync(request);
        }

        // --- EVENTS & CATEGORIES ---[cite: 17, 18]
        public async Task<List<EventViewModel>> GetEventsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/events");
            AttachApiSessionCookie(request);
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new List<EventViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<EventViewModel>>(json, _jsonOptions) ?? new();
        }

        public async Task<EventViewModel?> GetEventByIdAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/events/{id}");
            AttachApiSessionCookie(request);
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<EventViewModel>(json, _jsonOptions);
        }

        public async Task<EventViewModel?> CreateEventAsync(EventViewModel model)
        {
            var body = new
            {
                Name = model.EventName,
                Description = model.RouteInfo ?? "",
                EventDate = model.EventDate,
                Location = model.Location,
                Distance = model.Distance,
                EventType = model.EventType
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "api/events")
            {
                Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            };
            AttachApiSessionCookie(request);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            var created = JsonSerializer.Deserialize<EventViewModel>(json, _jsonOptions);

            if (created != null && model.BannerFile != null)
            {
                await UploadEventBannerAsync(created.EventId, model.BannerFile);
            }

            return created;
        }

        public async Task<bool> UpdateEventAsync(int id, EventViewModel model)
        {
            var body = new
            {
                Name = model.EventName,
                Description = model.RouteInfo ?? "",
                EventDate = model.EventDate,
                Location = model.Location,
                Distance = model.Distance,
                EventType = model.EventType
            };

            var request = new HttpRequestMessage(HttpMethod.Put, $"api/events/{id}")
            {
                Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            };
            AttachApiSessionCookie(request);

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return false;

            if (model.BannerFile != null)
            {
                await UploadEventBannerAsync(id, model.BannerFile);
            }
            return true;
        }

        public async Task<bool> DeleteEventAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, $"api/events/{id}");
            AttachApiSessionCookie(request);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task UploadEventBannerAsync(int eventId, IFormFile file)
        {
            using var content = new MultipartFormDataContent();
            using var stream = file.OpenReadStream();
            var fileContent = new StreamContent(stream);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.FileName);

            var request = new HttpRequestMessage(HttpMethod.Post, $"api/events/{eventId}/banner") { Content = content };
            AttachApiSessionCookie(request);
            await _httpClient.SendAsync(request);
        }

        public async Task<bool> AddCategoryAsync(int eventId, CategoryViewModel model)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"api/events/{eventId}/categories")
            {
                Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
            };
            AttachApiSessionCookie(request);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // --- ENROLMENTS ---[cite: 17, 18]
        public async Task<List<EnrolmentViewModel>> GetOrganiserEnrolmentsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/enrolments");
            AttachApiSessionCookie(request);
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new List<EnrolmentViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<EnrolmentViewModel>>(json, _jsonOptions) ?? new();
        }

        public async Task<(bool Success, string Message)> EnrolInEventAsync(int eventId, int categoryId)
        {
            var body = new { EventId = eventId, CategoryId = categoryId };
            var request = new HttpRequestMessage(HttpMethod.Post, "api/enrolments")
            {
                Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            };
            AttachApiSessionCookie(request);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode) return (true, "Successfully enrolled!");

            var err = await response.Content.ReadAsStringAsync();
            return (false, err);
        }

        // --- RESULTS & PROFILE ---[cite: 17, 18]
        public async Task<List<ResultViewModel>> GetMyResultsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/results/my-results");
            AttachApiSessionCookie(request);
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return new List<ResultViewModel>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<ResultViewModel>>(json, _jsonOptions) ?? new();
        }

        public async Task<bool> CaptureResultAsync(ResultViewModel model)
        {
            var body = new
            {
                EnrolmentId = model.EnrolmentId,
                RaceNumber = model.RaceNumber,
                FinishTime = model.FinishTime,
                PositionOverall = model.PositionOverall,
                PositionCategory = model.PositionCategory
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "api/results")
            {
                Content = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json")
            };
            AttachApiSessionCookie(request);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<ProfileViewModel?> GetProfileAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/users/profile");
            AttachApiSessionCookie(request);
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ProfileViewModel>(json, _jsonOptions);
        }

        public async Task<bool> UpdateProfileAsync(ProfileViewModel model)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, "api/users/profile")
            {
                Content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json")
            };
            AttachApiSessionCookie(request);
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode) return false;

            if (model.ProfilePictureFile != null)
            {
                using var content = new MultipartFormDataContent();
                using var stream = model.ProfilePictureFile.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.ProfilePictureFile.ContentType);
                content.Add(fileContent, "file", model.ProfilePictureFile.FileName);

                var picRequest = new HttpRequestMessage(HttpMethod.Post, "api/users/profile/picture") { Content = content };
                AttachApiSessionCookie(picRequest);
                await _httpClient.SendAsync(picRequest);
            }
            return true;
        }
    }
}