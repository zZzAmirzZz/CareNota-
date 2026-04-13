namespace CareNota.DTOs.Visit;

// ── Read ──────────────────────────────────────────────────────────────────────
public class VisitDto
{
    public int VisitID { get; set; }
    public DateTime VisitDate { get; set; }
    public string Subjective { get; set; } = string.Empty;
    public string Objective { get; set; } = string.Empty;
    public string Assessment { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
    public int AppointmentID { get; set; }
    public string PatientName { get; set; } = string.Empty;
}

public class VisitDetailDto : VisitDto
{
    public IList<DiagnosisSummaryDto> Diagnoses { get; set; } = [];
    public PrescriptionSummaryDto? Prescription { get; set; }
    public IList<LabTestSummaryDto> LabTests { get; set; } = [];
    public IList<AISummarySummaryDto> AISummaries { get; set; } = [];
}

public class DiagnosisSummaryDto
{
    public string ICD10Code { get; set; } = string.Empty;
    public string DiagnosisName { get; set; } = string.Empty;
}

public class PrescriptionSummaryDto
{
    public int PrescriptionID { get; set; }
    public string Instructions { get; set; } = string.Empty;
    public IList<PrescriptionMedicationSummaryDto> Medications { get; set; } = [];
}

public class PrescriptionMedicationSummaryDto
{
    public int MedicationID { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}

public class LabTestSummaryDto
{
    public int LabTestID { get; set; }
    public string LabTestName { get; set; } = string.Empty;
    public string TestResultURL { get; set; } = string.Empty;
    public bool HasResult => !string.IsNullOrEmpty(TestResultURL);
}

public class AISummarySummaryDto
{
    public int AISummaryID { get; set; }
    public string SummaryText { get; set; } = string.Empty;
    public string SummaryType { get; set; } = string.Empty;
    public float DoctorRating { get; set; }
}

// ── Write ─────────────────────────────────────────────────────────────────────
public class CreateVisitDto
{
    public DateTime VisitDate { get; set; } = DateTime.UtcNow;
    public string Subjective { get; set; } = string.Empty;
    public string Objective { get; set; } = string.Empty;
    public string Assessment { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
    public int AppointmentID { get; set; }
}

public class UpdateVisitDto
{
    public string Subjective { get; set; } = string.Empty;
    public string Objective { get; set; } = string.Empty;
    public string Assessment { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
}