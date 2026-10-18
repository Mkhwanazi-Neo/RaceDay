using System.ComponentModel.DataAnnotations;

namespace RaceDay.API.DTOs
{
    public class CreateEnrolmentDto
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }



    public class EnrolmentResponseDto
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

    public class CreateResultDto
    {
        [Required]
        public int EnrolmentId { get; set; }

        [Required]
        public int RaceNumber { get; set; }

        public TimeSpan FinishTime { get; set; }
        public int PositionOverall { get; set; }
        public int PositionCategory { get; set; }
    }

    public class ResultResponseDto
    {
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int RaceNumber { get; set; }
        public TimeSpan? FinishTime { get; set; }
        public int? PositionOverall { get; set; }
        public int? PositionCategory { get; set; }
    }

}

