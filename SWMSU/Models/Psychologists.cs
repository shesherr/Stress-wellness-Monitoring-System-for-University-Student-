using System.ComponentModel.DataAnnotations;

namespace SWMSU.Models
{
    public class Psychologists
    {
        public int PsychoId { get; set; }

        [Required]
        public string PsychoName { get; set; }

        [Required, EmailAddress]
        public string PsychoEmail { get; set; }

        [Required]
        public string PsychoPassword { get; set; }

        public string PsychoPhone { get; set; }

        public string Qualification { get; set; }

        public string Specialization { get; set; }

        public int Experience { get; set; }

        public string Role { get; set; }

        public string PsychoPermanentAddress { get; set; }
    }
}
