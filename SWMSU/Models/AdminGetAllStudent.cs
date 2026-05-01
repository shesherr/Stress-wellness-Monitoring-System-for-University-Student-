namespace SWMSU.Models
{
    public class AdminGetAllStudent
    {
        // ===== Booking =====
        public int BookingId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentDay { get; set; }
        public string AppointmentTime { get; set; }
        public string BookingSerial { get; set; }
        public string ApprovalStatus { get; set; }
        public DateTime CreatedAt { get; set; }

        // ===== Student (Users + StudentProfile) =====
        public int StudentUserId { get; set; }
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string StudentPhone { get; set; }

        public string StudentId { get; set; }
        public string Department { get; set; }
        public string Semester { get; set; }
        public decimal CGPA { get; set; }
        public int Credit { get; set; }
        public string ImageUrl { get; set; }

        // ===== Psychologist (Users + PsychologyProfile) =====
        public int PsychologistId { get; set; }
        public string PsychologistName { get; set; }
        public string PsychologistEmail { get; set; }
        public string PsychologistPhone { get; set; }

        public string Specialization { get; set; }
        public string Qualification { get; set; }
        public int ExperienceYears { get; set; }
        public string SeverityType { get; set; }
        public string Room { get; set; }
    }
}
