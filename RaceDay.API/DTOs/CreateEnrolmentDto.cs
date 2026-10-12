namespace RaceDayAPI.DTOs
{
    public class CreateEnrolmentDto
    {
        public int CategoryID { get; set; }
        public string? VolunteerRole { get; set; }
        public string? MedicalNotes { get; set; }
    }
}