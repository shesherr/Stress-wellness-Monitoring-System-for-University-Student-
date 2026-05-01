namespace SWMSU.Models
{
    public class PrescriptionVM
    {
        public int MedicinePrescriptionId { get; set; }
        public int PrescriptionId { get; set; }

        public int MedicineId { get; set; }
        public int BookingId { get; set; }
        public int Age { get; set; }
        public int UserId { get; set; }
        public string Diagnosed { get; set; }
        public string Address { get; set; }

        public string Specialization { get; set; }
        public string PatientName { get; set; }
        public string StudentName { get; set; }
        public string PsychiatristName { get; set; }
        public string BookingSerial { get; set; }

        //public DateTime AppointmentDay { get; set; }
        public string MedicineDuration { get; set; }
        public string Medicine_Notes { get; set; }

        public string MedicineDose { get; set; }
        public string MedicineName { get; set; }
        public string Qualification { get; set; }

        public string Frequency { get; set; }
        public string Advice { get; set; }

    }
}
