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
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public ResultsController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet("my-results")]
        [SessionAuthorize("Participant")]
        public async Task<IActionResult> GetMyResults()
        {
            var participantId = HttpContext.Session.GetInt32("UserId")!.Value;

            var results = await _context.Results
                .Include(res => res.Registration)
                .ThenInclude(r => r!.Event)
                .Include(res => res.Registration)
                .ThenInclude(r => r!.Category)
                .Where(res => res.Registration!.ParticipantId == participantId)
                .Select(res => new ResultResponseDto
                {
                    ResultId = res.ResultId,
                    EnrolmentId = res.RegistrationId,
                    EventName = res.Registration!.Event!.EventName,
                    CategoryName = res.Registration.Category!.CategoryName,
                    RaceNumber = res.RaceNumber,
                    FinishTime = res.FinishTime,
                    PositionOverall = res.PositionOverall,
                    PositionCategory = res.PositionCategory
                }).ToListAsync();

            return Ok(results);
        }

        [HttpPost]
        [SessionAuthorize("Organiser")]
        public async Task<IActionResult> CreateResult([FromBody] CreateResultDto dto)
        {
            var enrolment = await _context.Registrations.FindAsync(dto.EnrolmentId);
            if (enrolment == null) return NotFound(new { message = "Enrolment record not found." });

            var result = new Result
            {
                RegistrationId = dto.EnrolmentId,
                RaceNumber = dto.RaceNumber,
                FinishTime = dto.FinishTime,
                PositionOverall = dto.PositionOverall,
                PositionCategory = dto.PositionCategory
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return StatusCode(201, result);
        }
    }
}
