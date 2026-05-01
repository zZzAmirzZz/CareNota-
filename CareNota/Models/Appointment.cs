namespace CareNota.Models;

public enum AppointmentStatus { Scheduled, Completed, Cancelled }

public class Appointment
{
    public int AppointmentID { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Scheduled;
    public string AppointmentType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    // FKs
    public int PatientID { get; set; }
    public int ReceptionistID { get; set; }
    public int DoctorID { get; set; }

    // Navigation
    public Patient Patient { get; set; } = null!;
    public Receptionist Receptionist { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;

  

    public Visit? Visit { get; set; }
    public ICollection<Reminder> Reminders { get; set; } = [];
}