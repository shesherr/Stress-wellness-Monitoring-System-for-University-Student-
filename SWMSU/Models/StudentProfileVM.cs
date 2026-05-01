namespace SWMSU.Models
{
    public class StudentProfileVM
    {
        public int StudentProfileId { get; set; }
        public int UserId { get; set; }

        // Users table
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? BloodGroup { get; set; }

        // StudentProfile table
        public string? StudentId { get; set; }
        public string? Semester { get; set; }
        public decimal? CGPA { get; set; }
        public int? Credit { get; set; }
        public string? Department { get; set; }
        public string? Imageurl { get; set; }

        // File upload
        public IFormFile? ImageFile { get; set; }
    }
}
