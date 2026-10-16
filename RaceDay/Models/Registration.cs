namespace RaceDay.API.Models
{
    public class Registration
    {
        public int RegistrationId { get; set; }
        public int ParticipantId { get; set; }
        public int EventId { get; set; }
        public int CategoryId { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public string PaymentStatus { get; set; } = "Pending"; // Pending, Completed, Cancelled

        public Participant? Participant { get; set; }
        public Event? Event { get; set; }
        public Category? Category { get; set; }
        public Result? Result { get; set; }
    }
}
