namespace CareNota.DTOs.Medication;

// ── Read ──────────────────────────────────────────────────────────────────────
public class MedicationDto
{
    public int MedicationID { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string MedicationType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Strength { get; set; } = string.Empty;
}

// ── Write ─────────────────────────────────────────────────────────────────────
public class CreateMedicationDto
{
    public string MedicationName { get; set; } = string.Empty;
    public string MedicationType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Strength { get; set; } = string.Empty;
}

public class UpdateMedicationDto
{
    public string MedicationType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Strength { get; set; } = string.Empty;
}