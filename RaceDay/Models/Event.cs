namespace RaceDay.API.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public int OrganiserId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string? RouteInfo { get; set; }

        public Organiser? Organiser { get; set; }
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    }
}
