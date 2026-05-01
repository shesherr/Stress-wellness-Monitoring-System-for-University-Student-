using System.ComponentModel.DataAnnotations;

namespace SWMSU.Models
{
    public class UserVM
    {
        [Key]
        public int UserId { get; set; }
        public string? UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        public string Email { get; set; }
        public int UsertypeId { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        
        public int Age { get; set; }
        public string? Address { get; set; }
        public string? BloodGroup { get; set; }
        public string? Status { get; set; }
     

    }
}