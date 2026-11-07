namespace RaceDay.Web.Models
{
    public class EnrolmentViewModel
    {
        public int EnrolmentID { get; set; }
        public DateTime EnrolmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string? BibNumber { get; set; }
        public EventViewModel? Event { get; set; }
        public CategoryViewModel? Category { get; set; }
    }

    public class CategoryViewModel
    {
        public int CategoryID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string StartTime { get; set; } = string.Empty;
    }
}
