namespace SWMSU.Models
{
    public class PsychologyProfile
    {
        public int PsychologistId { get; set; }
        public int PsychiatristId { get; set; }
        public int UserId { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public int Age { get; set; }
        public string Address { get; set; }
        public string BloodGroup { get; set; }

        public string Room { get; set; }
        public string SeverityType { get; set; }

        // 🔥 MUST MATCH SP COLUMN NAME
        public string? ImageUrl { get; set; }

        // File upload only (NOT mapped from DB)
        public IFormFile? ImageFile { get; set; }

        public string Qualification { get; set; }
        public string Specialization { get; set; }
        public int? ExperienceYears { get; set; }

        public DateTime CreatedAt { get; set; }
        public string Bio { get; set; }
    }
}
