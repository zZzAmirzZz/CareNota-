namespace CareNota.DTOs.Visit;

// ── Read ──────────────────────────────────────────────────────────────────────
public class VisitDto
{
    public int VisitID { get; set; }
    public DateTime VisitDate { get; set; }
    public int AppointmentID { get; set; }
    public string PatientName { get; set; } = string.Empty;

    // null = not yet documented (manual or AI)
    public string? Subjective { get; set; }
    public string? Objective { get; set; }
    public string? Assessment { get; set; }
    public string? Plan { get; set; }

    // Written on approval or manually
    public string? Symptoms { get; set; }
    public string? WhenToSeekHelp { get; set; }
    public string? FollowUp { get; set; }
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
    public int DiagnosisID { get; set; }
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
    public string SummaryType { get; set; } = string.Empty;
    public string SummaryText { get; set; } = string.Empty;
    public int? DoctorRating { get; set; }
    public string? DoctorFeedback { get; set; }
}

// ── Write ─────────────────────────────────────────────────────────────────────
public class CreateVisitDto
{
    public int AppointmentID { get; set; }
    public DateTime VisitDate { get; set; } = DateTime.UtcNow;

    // All optional — null on AI path, filled on manual path
    public string? Subjective { get; set; }
    public string? Objective { get; set; }
    public string? Assessment { get; set; }
    public string? Plan { get; set; }
    public string? Symptoms { get; set; }
}

// All nullable — only send what changed, existing values preserved
public class UpdateVisitDto
{
    public string? Subjective { get; set; }
    public string? Objective { get; set; }
    public string? Assessment { get; set; }
    public string? Plan { get; set; }
    public string? Symptoms { get; set; }
    public string? WhenToSeekHelp { get; set; }
    public string? FollowUp { get; set; }
}