namespace SWMSU.Models
{
    public class StudentAppointmentVM
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string StudentId { get; set; }
        public string Department { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentTime { get; set; }
        public string AppointmentStatus { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
