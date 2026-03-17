namespace CareNota.Models;

public class Appointment
{
    public int AppointmentID { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string AppointmentType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // FKs
    public int PatientID { get; set; }
    public int ReceptionistID { get; set; }

    // Navigation
    public Patient Patient { get; set; } = null!;
    public Receptionist Receptionist { get; set; } = null!;
    public Visit? Visit { get; set; }
    public ICollection<Reminder> Reminders { get; set; } = [];
}