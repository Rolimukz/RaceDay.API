namespace RaceDay.Web.Models
{
    public class EventDetailsViewModel
    {
        public int EventID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string? Venue { get; set; }
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public decimal EntryFee { get; set; }
        public string Status { get; set; } = string.Empty;

        // This holds the categories returned by the API
        public List<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();
    }
}