using Microsoft.AspNetCore.Mvc;
using RaceDay.MVC.Attributes;
using RaceDay.MVC.Models;
using RaceDay.MVC.Services;

namespace RaceDay.MVC.Controllers
{
    [MvcSessionAuthorize("Organiser")]
    public class OrganiserController : Controller
    {
        private readonly RaceDayApiService _apiService;

        public OrganiserController(RaceDayApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var userId = HttpContext.Session.GetInt32("UserId")!.Value;
            var allEvents = await _apiService.GetEventsAsync();
            var myEvents = allEvents.Where(e => e.OrganiserId == userId).ToList();
            var enrolments = await _apiService.GetOrganiserEnrolmentsAsync();

            var vm = new OrganiserDashboardViewModel
            {
                Events = myEvents,
                Enrolments = enrolments
            };
            return View(vm);
        }

        [HttpGet]
        public IActionResult CreateEvent() => View(new EventViewModel());

        [HttpPost]
        public async Task<IActionResult> CreateEvent(EventViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var created = await _apiService.CreateEventAsync(model);
            if (created == null)
            {
                ViewBag.Error = "Could not create event.";
                return View(model);
            }
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            var ev = await _apiService.GetEventByIdAsync(id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        [HttpPost]
        public async Task<IActionResult> EditEvent(int id, EventViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _apiService.UpdateEventAsync(id, model);
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            await _apiService.DeleteEventAsync(id);
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> ManageCategories(int eventId)
        {
            var ev = await _apiService.GetEventByIdAsync(eventId);
            if (ev == null) return NotFound();
            ViewBag.Event = ev;
            return View(new CategoryViewModel { EventId = eventId });
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(CategoryViewModel model)
        {
            await _apiService.AddCategoryAsync(model.EventId, model);
            return RedirectToAction("ManageCategories", new { eventId = model.EventId });
        }

        [HttpGet]
        public async Task<IActionResult> Enrolments(int? eventId)
        {
            var enrolments = await _apiService.GetOrganiserEnrolmentsAsync();
            if (eventId.HasValue)
            {
                enrolments = enrolments.Where(e => e.EventId == eventId.Value).ToList();
            }
            return View(enrolments);
        }

        [HttpGet]
        public IActionResult CaptureResult(int enrolmentId)
        {
            return View(new ResultViewModel { EnrolmentId = enrolmentId });
        }

        [HttpPost]
        public async Task<IActionResult> CaptureResult(ResultViewModel model)
        {
            await _apiService.CaptureResultAsync(model);
            TempData["Success"] = "Race result published!";
            return RedirectToAction("Enrolments");
        }
    }
}