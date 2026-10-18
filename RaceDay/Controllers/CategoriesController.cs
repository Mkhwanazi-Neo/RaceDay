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
    public class CategoriesController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public CategoriesController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SessionAuthorize]
        public async Task<IActionResult> GetCategories(int eventId)
        {
            var categories = await _context.Categories.Where(c => c.EventId == eventId).ToListAsync();
            return Ok(categories);
        }

        [HttpPost]
        [SessionAuthorize("Organiser")]
        public async Task<IActionResult> AddCategory(int eventId, [FromBody] CreateCategoryDto dto)
        {
            var raceEvent = await _context.Events.FindAsync(eventId);
            if (raceEvent == null) return NotFound(new { message = "Event not found." });

            var category = new Category
            {
                EventId = eventId,
                CategoryName = dto.CategoryName,
                Discipline = dto.Discipline,
                DistanceKm = dto.DistanceKm,
                EntryFee = dto.EntryFee
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return StatusCode(201, category);
        }
    }
}
