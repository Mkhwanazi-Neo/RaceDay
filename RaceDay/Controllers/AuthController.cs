using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.Models;
using RaceDay.API.Services;

namespace RaceDay.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayDbContext _context;

        public AuthController(RaceDayDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Registers a new Organiser or Participant account.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string role = dto.Role.Trim().ToLower();

            if (role != "organiser" && role != "participant")
            {
                return BadRequest(new { message = "Role must be either 'Organiser' or 'Participant'." });
            }

            string hashedPassword = PasswordHasher.HashPassword(dto.Password);

            if (role == "organiser")
            {
                if (await _context.Organisers.AnyAsync(o => o.Email == dto.Email))
                    return BadRequest(new { message = "Email is already registered as an Organiser." });

                var organiser = new Organiser
                {
                    Name = $"{dto.FirstName} {dto.LastName}".Trim(),
                    Email = dto.Email,
                    PasswordHash = hashedPassword,
                    Phone = dto.Phone
                };

                _context.Organisers.Add(organiser);
                await _context.SaveChangesAsync();

                return StatusCode(201, new UserResponseDto
                {
                    UserId = organiser.OrganiserId,
                    Name = organiser.Name,
                    Email = organiser.Email,
                    Role = "Organiser"
                });
            }
            else
            {
                if (await _context.Participants.AnyAsync(p => p.Email == dto.Email))
                    return BadRequest(new { message = "Email is already registered as a Participant." });

                if (dto.DateOfBirth == null)
                    return BadRequest(new { message = "Date of Birth is required for Participants." });

                var participant = new Participant
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,
                    PasswordHash = hashedPassword,
                    Gender = dto.Gender,
                    DateOfBirth = dto.DateOfBirth.Value
                };

                _context.Participants.Add(participant);
                await _context.SaveChangesAsync();

                return StatusCode(201, new UserResponseDto
                {
                    UserId = participant.ParticipantId,
                    Name = $"{participant.FirstName} {participant.LastName}",
                    Email = participant.Email,
                    Role = "Participant"
                });
            }
        }

        /// <summary>
        /// Logs in an existing user and stores their identity in session.
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Check Organisers table
            var organiser = await _context.Organisers.FirstOrDefaultAsync(o => o.Email == dto.Email);
            if (organiser != null && PasswordHasher.VerifyPassword(dto.Password, organiser.PasswordHash))
            {
                // Set Session Keys
                HttpContext.Session.SetInt32("UserId", organiser.OrganiserId);
                HttpContext.Session.SetString("UserRole", "Organiser");
                HttpContext.Session.SetString("UserEmail", organiser.Email);

                return Ok(new UserResponseDto
                {
                    UserId = organiser.OrganiserId,
                    Name = organiser.Name,
                    Email = organiser.Email,
                    Role = "Organiser"
                });
            }

            // Check Participants table
            var participant = await _context.Participants.FirstOrDefaultAsync(p => p.Email == dto.Email);
            if (participant != null && PasswordHasher.VerifyPassword(dto.Password, participant.PasswordHash))
            {
                // Set Session Keys
                HttpContext.Session.SetInt32("UserId", participant.ParticipantId);
                HttpContext.Session.SetString("UserRole", "Participant");
                HttpContext.Session.SetString("UserEmail", participant.Email);

                return Ok(new UserResponseDto
                {
                    UserId = participant.ParticipantId,
                    Name = $"{participant.FirstName} {participant.LastName}",
                    Email = participant.Email,
                    Role = "Participant"
                });
            }

            return Unauthorized(new { message = "Invalid email or password." });
        }

        /// <summary>
        /// Logs out the current session.
        /// </summary>
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Logged out successfully." });
        }
    }
}
