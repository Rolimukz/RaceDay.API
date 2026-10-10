using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDayAPI.Models
{
    public class Enrolment
    {
        [Key]
        public int EnrolmentID { get; set; }
        public int UserID { get; set; }
        public int EventID { get; set; }
        public int CategoryID { get; set; }
        public DateTime EnrolmentDate { get; set; }
        public string? Status { get; set; }
        public string? PaymentStatus { get; set; }
        public string? BibNumber { get; set; }
        public string? VolunteerRole { get; set; }
        public string? MedicalNotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User? User { get; set; }
        public Event? Event { get; set; }
        public Category? Category { get; set; }
        public Result? Result { get; set; }
    }
}