using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Attributes;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EventsController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SessionAuthorize]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _context.Events.Include(e => e.Categories).ToListAsync();
            return Ok(events);
        }

        [HttpGet("{id}")]
        [SessionAuthorize]
        public async Task<IActionResult> GetEventById(int id)
        {
            var raceEvent = await _context.Events.Include(e => e.Categories).FirstOrDefaultAsync(e => e.EventId == id);
            if (raceEvent == null) return NotFound(new { message = "Event not found." });
            return Ok(raceEvent);
        }

        [HttpPost]
        [SessionAuthorize("Organiser")]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto dto)
        {
            var organiserId = HttpContext.Session.GetInt32("UserId")!.Value;

            var newEvent = new Event
            {
                OrganiserId = organiserId,
                EventName = dto.Name,
                EventDate = dto.EventDate,
                Location = dto.Location,
                RouteInfo = dto.Description
            };

            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEventById), new { id = newEvent.EventId }, newEvent);
        }

        [HttpPut("{id}")]
        [SessionAuthorize("Organiser")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] CreateEventDto dto)
        {
            var raceEvent = await _context.Events.FindAsync(id);
            if (raceEvent == null) return NotFound(new { message = "Event not found." });

            raceEvent.EventName = dto.Name;
            raceEvent.EventDate = dto.EventDate;
            raceEvent.Location = dto.Location;
            raceEvent.RouteInfo = dto.Description;

            await _context.SaveChangesAsync();
            return Ok(raceEvent);
        }

        [HttpDelete("{id}")]
        [SessionAuthorize("Organiser")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var raceEvent = await _context.Events.FindAsync(id);
            if (raceEvent == null) return NotFound(new { message = "Event not found." });

            _context.Events.Remove(raceEvent);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Event deleted successfully." });
        }
    }
}
