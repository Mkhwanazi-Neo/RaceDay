using System.ComponentModel.DataAnnotations;


namespace RaceDay.MVC.Models
{
    public class RegisterViewModel
    {
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required, MinLength(6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Participant"; // Organiser or Participant[cite: 15, 16]

        public string? Phone { get; set; }
        public string? Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
    }

    public class LoginViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }

    public class UserSessionDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class EventViewModel
    {
        public int EventId { get; set; }
        public int OrganiserId { get; set; }

        [Required]
        public string EventName { get; set; } = string.Empty;

        [Required, DataType(DataType.Date)]
        public DateTime EventDate { get; set; } = DateTime.Today.AddDays(14);

        [Required]
        public string Location { get; set; } = string.Empty;

        public string? RouteInfo { get; set; }
        public string? BannerImageUrl { get; set; } //[cite: 17, 18]
        public decimal Distance { get; set; } //[cite: 17]
        public string EventType { get; set; } = "Run"; //[cite: 17]

        public IFormFile? BannerFile { get; set; } //[cite: 17, 18]
        public List<CategoryViewModel> Categories { get; set; } = new();
    }

    public class CategoryViewModel
    {
        public int CategoryId { get; set; }
        public int EventId { get; set; }

        [Required]
        public string CategoryName { get; set; } = string.Empty;

        [Required]
        public string Discipline { get; set; } = "Running";

        [Required]
        public decimal DistanceKm { get; set; }

        public decimal EntryFee { get; set; }
    }

    public class EnrolmentViewModel
    {
        public int EnrolmentId { get; set; }
        public int ParticipantId { get; set; }
        public string ParticipantName { get; set; } = string.Empty;
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
        public string PaymentStatus { get; set; } = string.Empty;
    }

    public class ResultViewModel
    {
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int RaceNumber { get; set; }
        public TimeSpan? FinishTime { get; set; }
        public int? PositionOverall { get; set; }
        public int? PositionCategory { get; set; }
        public int TotalFinishers { get; set; } //[cite: 18]
    }

    public class ProfileViewModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public string? ProfilePictureUrl { get; set; } //[cite: 18]
        public IFormFile? ProfilePictureFile { get; set; } //[cite: 18]
    }

    public class OrganiserDashboardViewModel
    {
        public List<EventViewModel> Events { get; set; } = new();
        public List<EnrolmentViewModel> Enrolments { get; set; } = new();
        public int TotalEvents => Events.Count;
        public int TotalEnrolments => Enrolments.Count;
    }
}

