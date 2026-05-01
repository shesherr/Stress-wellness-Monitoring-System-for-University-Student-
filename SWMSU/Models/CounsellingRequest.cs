using System;

namespace SWMSU.Models
{
    public class CounsellingRequest
    {
        public int RequestId { get; set; }
        public int BookingId { get; set; }
        public int SenderUserId { get; set; }
        public int ReceiverUserId { get; set; }

        // 🧠 Psychologist Profile
        public string Bio { get; set; }
        public int ExperienceYears { get; set; }
        public string Qualification { get; set; }
        public string Specialization { get; set; }
        public string SeverityType { get; set; }
        public string Room { get; set; }

        // 🧑‍🎓 Student (Sender) info
        public string StudentName { get; set; }
        public string StudentEmail { get; set; }
        public string StudentPhone { get; set; }

        // 👨‍⚕️ Psychologist (Receiver) info
        public string PsychologistName { get; set; }
        public string PsychologistEmail { get; set; }
        public string PsychologistPhone { get; set; }

        // 🎓 Student Profile
        public string StudentId { get; set; }
        public string Department { get; set; }
        public string Semester { get; set; }
        public decimal CGPA { get; set; }
        public int Credit { get; set; }

        // 📝 Counselling Request
        public string Message { get; set; }
        public string ApprovalStatus { get; set; }
        public DateTime CreatedAt { get; set; } 

        // 📅 Booking Info (NEW – SQL match)
        public DateTime? AppointmentDate { get; set; }
        public string AppointmentDay { get; set; }
        public string AppointmentTime { get; set; }
        public string BookingSerial { get; set; }
    }
}
