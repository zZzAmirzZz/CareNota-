using System.ComponentModel.DataAnnotations;

namespace CareNota.Models;

public class Reminder
{
    [Key]
    public int ReminderID { get; set; }
    public string Message { get; set; } = string.Empty;
    public string ReminderType { get; set; } = string.Empty;
    public DateTime ReminderDateTime { get; set; }

    // FKs
    public int PatientID { get; set; }
    public int? PrescriptionID { get; set; }   // null if it's an appointment reminder
    public int? AppointmentID { get; set; }    // null if it's a medication reminder

    // Navigation
    public Patient Patient { get; set; } = null!;
    public Prescription Prescription { get; set; } = null!;
    public Appointment Appointment { get; set; } = null!;
}