using System.ComponentModel.DataAnnotations;

namespace SWMSU.Models
{
    public class Dass21Form
    {
        public int FormId { get; set; }

       
   


        public DateTime Date { get; set; } = DateTime.Now;
        public int UserId { get; set; }
        public int Q1 { get; set; }
        public int Q2 { get; set; }
        public int Q3 { get; set; }
        public int Q4 { get; set; }
        public int Q5 { get; set; }
        public int Q6 { get; set; }
        public int Q7 { get; set; }
        public int Q8 { get; set; }
        public int Q9 { get; set; }
        public int Q10 { get; set; }
        public int Q11 { get; set; }
        public int Q12 { get; set; }
        public int Q13 { get; set; }
        public int Q14 { get; set; }
        public int Depression { get; set; }
        public int Stress { get; set; }
    }
}
