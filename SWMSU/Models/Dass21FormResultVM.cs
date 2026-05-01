namespace SWMSU.Models
{
    public class Dass21FormResultVM
    {

        public int FormId { get; set; }
        public string StudentVarsityId { get; set; }   // StudentProfile.StudentId
            public string UserName { get; set; }        // Users.UserName
            public int Semester { get; set; }       // StudentProfile.Semester
            public string Email { get; set; }       // Users.Email
        public string Address { get; set; }
        public int Age { get; set; }                   // Users.Age
            public string? BloodGroup { get; set; }        // Users.BloodGroup
            public DateTime UserCreateProfile { get; set; }    // Users.CreatedAt
            public DateTime FormCreateTime { get; set; }    // Dass21Forms.CreatedAt
            public string? Department { get; set; }        // StudentProfile.Department
        public int Depression { get; set; }
        public int Stress { get; set; }
        // 🔥 NEW
        public string SeverityType { get; set; }
    }
    }




