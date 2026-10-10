using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Models
{
    public class EventRoute
    {
        [Key]
        public int RouteID { get; set; }
        public int EventID { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public string? StartPoint { get; set; }
        public string? EndPoint { get; set; }
        public decimal Distance { get; set; }
        public decimal? ElevationGain { get; set; }
        public string? RouteMapURL { get; set; }
        public string? GPXFileURL { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public Event? Event { get; set; }
    }
}