namespace RaceDayAPI.DTOs
{
    public class UpdateEventDto
    {
        public string? EventName { get; set; }
        public string? Description { get; set; }
        public DateTime? EventDate { get; set; }
        public string? Venue { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public decimal? EntryFee { get; set; }
        public int? MaxParticipants { get; set; }
        public string? Status { get; set; }
    }
}
