namespace SWMSU.Models
{
    public class PsychiatristSchedule
    {
        public int PsychiatristScheduleId { get; set; }
        public int PsychiatristId { get; set; }

        public string StartDay { get; set; }
        public string EndDay { get; set; }

        public string StartTime { get; set; }   // 🔴 TimeSpan না
        public string EndTime { get; set; }

        public string Status { get; set; }
    }
}
