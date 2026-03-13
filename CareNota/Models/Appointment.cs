namespace CareNota.Models
{
    public class Appointment
    {
        public int AppointmentID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; }
        public string AppointmentType { get; set; }
        public DateTime CreatedAt { get; set; }

        public int PatientID { get; set; } // Foreign key to Patient
        public int ReceptionistID { get; set; } // Foreign key to Receptionist
    }
}
