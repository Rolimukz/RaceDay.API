using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Models
{
    public class Result
    {
        [Key]
        public int ResultID { get; set; }
        public int EnrolmentID { get; set; }
        public int UserID { get; set; }
        public int EventID { get; set; }
        public int CategoryID { get; set; }
        public TimeSpan? FinishTime { get; set; }
        public int? OverallPosition { get; set; }
        public int? CategoryPosition { get; set; }
        public string? Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Enrolment? Enrolment { get; set; }
        public User? User { get; set; }
        public Event? Event { get; set; }
        public Category? Category { get; set; }
    }
}