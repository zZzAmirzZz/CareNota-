namespace CareNota.DTOs.Visit;

// ── Read ──────────────────────────────────────────────────────────────────────

public class VisitDto
{
    public int VisitID { get; set; }
    public DateTime VisitDate { get; set; }
    public int AppointmentID { get; set; }
    public string PatientName { get; set; } = string.Empty;

    // null = visit opened but not yet documented (neither manually nor via AI)
    public string? Subjective { get; set; }
    public string? Objective { get; set; }
    public string? Assessment { get; set; }
    public string? Plan { get; set; }

    // Written on AI summary approval — null if doctor used manual entry
    // or if AI flow not yet approved
    public string? WhenToSeekHelp { get; set; }
    public DateTime? FollowUpDate { get; set; }
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

// DoctorRating removed — no longer part of AISummary model
public class AISummarySummaryDto
{
    public int AISummaryID { get; set; }
    public string SummaryType { get; set; } = string.Empty;
    public string SummaryText { get; set; } = string.Empty;
}

// ── Write ─────────────────────────────────────────────────────────────────────

// Doctor creates a visit when the appointment starts.
// SOAP fields are optional at creation:
//   - Manual path: doctor fills them right away (or later via PUT)
//   - AI path:     doctor leaves them empty and uses audio recording instead
public class CreateVisitDto
{
    public int AppointmentID { get; set; }
    public DateTime VisitDate { get; set; } = DateTime.UtcNow;

    // Optional at creation — doctor can fill now or fill later via PUT or AI flow
    public string? Subjective { get; set; }
    public string? Objective { get; set; }
    public string? Assessment { get; set; }
    public string? Plan { get; set; }
}

// Doctor manually updates visit details.
// All fields nullable — only send what changed, existing values are preserved.
// WhenToSeekHelp and FollowUpDate can also be set manually here,
// not just through the AI approval flow.
public class UpdateVisitDto
{
    public string? Subjective { get; set; }
    public string? Objective { get; set; }
    public string? Assessment { get; set; }
    public string? Plan { get; set; }
    public string? WhenToSeekHelp { get; set; }
    public DateTime? FollowUpDate { get; set; }
}