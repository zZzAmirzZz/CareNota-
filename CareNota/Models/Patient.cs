namespace CareNota.Models;

public class Patient
{
    public int PatientID { get; set; }
    public string Gender { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string BloodType { get; set; } = string.Empty;
    public string Allergies { get; set; } = string.Empty;
    public string InsuranceInfo { get; set; } = string.Empty;

    // FK → ApplicationUser (string GUID)
    public string UserId { get; set; } = string.Empty;

    // Navigation
    public ApplicationUser User { get; set; } = null!;
    public MedicalHistory? MedicalHistory { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = [];
    public ICollection<Reminder> Reminders { get; set; } = [];
}