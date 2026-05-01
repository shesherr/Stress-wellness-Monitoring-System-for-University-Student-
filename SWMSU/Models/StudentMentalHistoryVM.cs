namespace SWMSU.Models
{
    public class StudentMentalHistoryVM
    {
        public string StudentName { get; set; }
        public string Semester { get; set; }
        public int Credit { get; set; }

        // Mental health (DASS-21)
        public string Depression { get; set; }
        public string Stress { get; set; }
        public DateTime Dass21FormDate { get; set; }

        // Booking
        public string BookingSerial { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentTime { get; set; }
        public string AppointmentDay { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Psychologist
        public string PsychologistName { get; set; }
        public string PsychologyImage { get; set; }
    }
}
