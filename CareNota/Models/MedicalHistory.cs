namespace CareNota.Models;

public class MedicalHistory
{
    public int MedicalHistoryId { get; set; }
    public string ChiefComplaint { get; set; } = string.Empty;
    public string PresentIllness { get; set; } = string.Empty;
    public string PastMedicalHistory { get; set; } = string.Empty;

    // FK
    public int PatientId { get; set; }

    // Navigation
    public Patient Patient { get; set; } = null!;
}