using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Attributes;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    [MvcSessionAuthorize("Participant")]
    public class ParticipantController : Controller
    {
        private readonly RaceDayApiService _apiService;

        public ParticipantController(RaceDayApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(string? search, string? location)
        {
            var events = await _apiService.GetEventsAsync();

            if (!string.IsNullOrWhiteSpace(search))
                events = events.Where(e => e.EventName.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!string.IsNullOrWhiteSpace(location))
                events = events.Where(e => e.Location.Contains(location, StringComparison.OrdinalIgnoreCase)).ToList();

            ViewBag.Search = search;
            ViewBag.Location = location;
            return View(events);
        }

        public async Task<IActionResult> EventDetails(int id)
        {
            var ev = await _apiService.GetEventByIdAsync(id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        [HttpPost]
        public async Task<IActionResult> Enrol(int eventId, int categoryId)
        {
            var (success, message) = await _apiService.EnrolInEventAsync(eventId, categoryId);
            if (success)
                TempData["Success"] = "You have successfully enrolled for this event!";
            else
                TempData["Error"] = "You are already enrolled in this event or the selection was invalid.";

            return RedirectToAction("EventDetails", new { id = eventId });
        }

        public async Task<IActionResult> MyResults()
        {
            var results = await _apiService.GetMyResultsAsync();
            return View(results);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var profile = await _apiService.GetProfileAsync();
            return View(profile ?? new ProfileViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            await _apiService.UpdateProfileAsync(model);
            TempData["Success"] = "Profile updated successfully!";
            return RedirectToAction("Profile");
        }
    }
}