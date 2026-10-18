using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RaceDay.API.Attributes;
using RaceDay.API.Data;
using RaceDay.API.DTOs;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public UsersController(RaceDayDbContext context)
        {
            _context = context;
        }

        [HttpGet("profile")]
        [SessionAuthorize]
        public async Task<IActionResult> GetProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == "Organiser")
            {
                var org = await _context.Organisers.FindAsync(userId);
                if (org == null) return NotFound();

                return Ok(new UserProfileDto
                {
                    UserId = org.OrganiserId,
                    FirstName = org.Name,
                    Email = org.Email,
                    Role = "Organiser",
                    Phone = org.Phone
                });
            }
            else
            {
                var part = await _context.Participants.FindAsync(userId);
                if (part == null) return NotFound();

                return Ok(new UserProfileDto
                {
                    UserId = part.ParticipantId,
                    FirstName = part.FirstName,
                    LastName = part.LastName,
                    Email = part.Email,
                    Role = "Participant",
                    Gender = part.Gender,
                    DateOfBirth = part.DateOfBirth
                });
            }
        }

        [HttpPut("profile")]
        [SessionAuthorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole == "Organiser")
            {
                var org = await _context.Organisers.FindAsync(userId);
                if (org == null) return NotFound();

                org.Name = $"{dto.FirstName} {dto.LastName}".Trim();
                org.Email = dto.Email;
                org.Phone = dto.Phone;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Organiser profile updated successfully." });
            }
            else
            {
                var part = await _context.Participants.FindAsync(userId);
                if (part == null) return NotFound();

                part.FirstName = dto.FirstName;
                part.LastName = dto.LastName;
                part.Email = dto.Email;
                part.Gender = dto.Gender;
                if (dto.DateOfBirth.HasValue) part.DateOfBirth = dto.DateOfBirth.Value;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Participant profile updated successfully." });
            }
        }
    }
}
