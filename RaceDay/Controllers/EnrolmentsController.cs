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
    public class EnrolmentsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public EnrolmentsController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [SessionAuthorize("Organiser")]
        public async Task<IActionResult> GetOrganiserEnrolments()
        {
            var organiserId = HttpContext.Session.GetInt32("UserId")!.Value;

            var enrolments = await _context.Registrations
                .Include(r => r.Participant)
                .Include(r => r.Event)
                .Include(r => r.Category)
                .Where(r => r.Event!.OrganiserId == organiserId)
                .Select(r => new EnrolmentResponseDto
                {
                    EnrolmentId = r.RegistrationId,
                    ParticipantId = r.ParticipantId,
                    ParticipantName = $"{r.Participant!.FirstName} {r.Participant.LastName}",
                    EventId = r.EventId,
                    EventName = r.Event!.EventName,
                    CategoryId = r.CategoryId,
                    CategoryName = r.Category!.CategoryName,
                    RegistrationDate = r.RegistrationDate,
                    PaymentStatus = r.PaymentStatus
                }).ToListAsync();

            return Ok(enrolments);
        }

        [HttpPost]
        [SessionAuthorize("Participant")]
        public async Task<IActionResult> CreateEnrolment([FromBody] CreateEnrolmentDto dto)
        {
            var participantId = HttpContext.Session.GetInt32("UserId")!.Value;

            bool alreadyRegistered = await _context.Registrations
                .AnyAsync(r => r.ParticipantId == participantId && r.EventId == dto.EventId);

            if (alreadyRegistered)
                return BadRequest(new { message = "Participant is already enrolled in this event." });

            var enrolment = new Registration
            {
                ParticipantId = participantId,
                EventId = dto.EventId,
                CategoryId = dto.CategoryId,
                PaymentStatus = "Completed"
            };

            _context.Registrations.Add(enrolment);
            await _context.SaveChangesAsync();

            return StatusCode(201, enrolment);
        }
    }
}
