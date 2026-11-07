using Microsoft.AspNetCore.Http;

namespace RaceDay.Web.Models
{
    public class CreateEventViewModel
    {
        public string EventName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string? Venue { get; set; }
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public decimal EntryFee { get; set; }
        public int MaxParticipants { get; set; }
        public string Status { get; set; } = "Draft";

        // This line is what allows the file upload
        public IFormFile? BannerImage { get; set; }
    }
}