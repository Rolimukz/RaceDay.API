namespace RaceDayAPI.DTOs
{
    public class UpdateCategoryDto
    {
        public string? CategoryName { get; set; }
        public decimal? Distance { get; set; }
        public TimeSpan? StartTime { get; set; }
        public string? AgeGroup { get; set; }
        public decimal? EntryFee { get; set; }
        public int? MaxParticipants { get; set; }
    }
}