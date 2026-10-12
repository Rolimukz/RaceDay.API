namespace RaceDayAPI.DTOs
{
    public class CreateCategoryDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public TimeSpan StartTime { get; set; }
        public string? AgeGroup { get; set; }
        public decimal EntryFee { get; set; }
        public int MaxParticipants { get; set; }
    }
}