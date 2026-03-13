namespace CareNota.Models
{
    public class Reminder
    {
        public int ReminderID { get; set; }
        public string Message { get; set; }
        public string ReminderType { get; set; }
        public DateTime ReminderDateTime { get; set; }

        public int PatientID { get; set; }

        public int PrescriptionID { get; set; }
        public int AppointmentID { get; set; }
    }
}
