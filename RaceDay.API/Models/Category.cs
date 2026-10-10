using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Models
{
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }
        public int EventID { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public TimeSpan StartTime { get; set; }
        public string? AgeGroup { get; set; }
        public decimal EntryFee { get; set; }
        public int MaxParticipants { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Event? Event { get; set; }
        public ICollection<Enrolment>? Enrolments { get; set; }
    }
}
