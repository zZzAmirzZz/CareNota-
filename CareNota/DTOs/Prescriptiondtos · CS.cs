namespace CareNota.DTOs.Prescription;

// ── Read ──────────────────────────────────────────────────────────────────────
public class PrescriptionDto
{
    public int PrescriptionID { get; set; }
    public string Instructions { get; set; } = string.Empty;
    public int VisitID { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public IList<PrescriptionMedicationDetailDto> Medications { get; set; } = [];
}

public class PrescriptionMedicationDetailDto
{
    public int MedicationID { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string MedicationType { get; set; } = string.Empty;
    public string Strength { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

// ── Write ─────────────────────────────────────────────────────────────────────
public class CreatePrescriptionDto
{
    public string Instructions { get; set; } = string.Empty;
    public int VisitID { get; set; }
}

public class AddMedicationToPrescriptionDto
{
    public int MedicationID { get; set; }
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class UpdatePrescriptionDto
{
    public string Instructions { get; set; } = string.Empty;
}