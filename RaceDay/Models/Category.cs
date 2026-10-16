namespace RaceDay.API.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        public int EventId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Discipline { get; set; } = string.Empty; // Running, Walking, Cycling
        public decimal DistanceKm { get; set; }
        public decimal EntryFee { get; set; }

        public Event? Event { get; set; }
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    }
}
