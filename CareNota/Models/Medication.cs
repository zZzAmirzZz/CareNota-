namespace CareNota.Models;

public class Medication
{
    public int MedicationID { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string MedicationType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Strength { get; set; } = string.Empty;

    // Navigation
    public ICollection<PrescriptionMedication> PrescriptionMedications { get; set; } = [];
}