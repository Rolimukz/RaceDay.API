namespace RaceDay.Web.Models
{
    public class EventViewModel
    {
        public int EventID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int TotalCategories { get; set; }
        public string? Organiser { get; set; }
    }
}