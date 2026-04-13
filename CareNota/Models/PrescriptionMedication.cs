using System.ComponentModel.DataAnnotations;

namespace CareNota.Models;

public class PrescriptionMedication
{
    // Composite PK (configured in DbContext)
    [Key]
    public int PrescriptionID { get; set; }
    public int MedicationID { get; set; }

    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    // Navigation
    public Prescription Prescription { get; set; } = null!;
    public Medication Medication { get; set; } = null!;
}