namespace RaceDay.API.Models
{
    public class Participant
    {
        public int ParticipantId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // Hashed password requirement
        public string? Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    }
}
