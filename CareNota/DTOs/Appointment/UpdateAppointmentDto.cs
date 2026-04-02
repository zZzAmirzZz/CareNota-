namespace CareNota.DTOs.Appointment
{
    public class UpdateAppointmentDto
    {
        public DateTime AppointmentDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string AppointmentType { get; set; } = string.Empty;
    }
}
