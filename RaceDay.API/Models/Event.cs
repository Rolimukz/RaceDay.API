using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Models
{
    public class Event
    {
        [Key]
        public int EventID { get; set; }
        public int OrganiserID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public string? Venue { get; set; }
        public string City { get; set; } = string.Empty;
        public string Province { get; set; } = string.Empty;
        public decimal EntryFee { get; set; }
        public int MaxParticipants { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User? Organiser { get; set; }
        public ICollection<Category>? Categories { get; set; }
        public ICollection<Enrolment>? Enrolments { get; set; }
        public EventRoute? EventRoute { get; set; }
    }
}