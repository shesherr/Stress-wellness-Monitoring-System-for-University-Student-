namespace SWMSU.Models
{
    public class Student
    {
       
            public int StudentId { get; set; }                       // Primary Key

            public string StudentVarsityId { get; set; }             // University ID

            public string StudentName { get; set; }                  // Full Name

        public int CurrentSemester { get; set; }           // Semester Info

        public string StudentEmail { get; set; }                 // Email

            public string StudentPassword { get; set; }              // Password (Hash recommended)

            public int CreditCurrent { get; set; }                   // Current Credits

            public string Role { get; set; }                         // Student/Psychologist/Admin (from SP)

            public string StudentPermanentAddress { get; set; }      // Permanent Address

            public string StudentBloodGroup { get; set; }            // Blood Group

            public string StudentPhone { get; set; }            // Profile Picture (Path)
        }

    }
