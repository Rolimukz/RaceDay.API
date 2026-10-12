namespace RaceDayAPI.DTOs
{
    public class CreateResultDto
    {
        public TimeSpan? FinishTime { get; set; }
        public int? OverallPosition { get; set; }
        public int? CategoryPosition { get; set; }
        public string? Status { get; set; }
    }
}