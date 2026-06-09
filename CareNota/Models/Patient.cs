using System.ComponentModel.DataAnnotations;

namespace CareNota.Models;

public class Patient


{
    [Key]
    public int PatientID { get; set; }
    public string? Gender { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string? BloodType { get; set; } = string.Empty;
    public string? Allergies { get; set; } = string.Empty;
    public string? InsuranceInfo { get; set; } = string.Empty;

    public string? ChronicConditions { get; set; }

    // FK → ApplicationUser (string GUID)
    public string UserId { get; set; } = string.Empty;

    // Navigation
    public ApplicationUser User { get; set; } = null!;
    
    public ICollection<Appointment> Appointments { get; set; } = [];
    public ICollection<Reminder> Reminders { get; set; } = [];
}