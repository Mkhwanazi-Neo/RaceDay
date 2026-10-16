namespace RaceDay.API.Models
{
    public class Result
    {
        public int ResultId { get; set; }
        public int RegistrationId { get; set; }
        public int RaceNumber { get; set; }
        public TimeSpan? FinishTime { get; set; }
        public int? PositionOverall { get; set; }
        public int? PositionCategory { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Registration? Registration { get; set; }
    }
}
