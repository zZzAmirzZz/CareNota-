namespace CareNota.DTOs.Diagnosis;

// ── Read ──────────────────────────────────────────────────────────────────────
public class DiagnosisDto
{
    public string ICD10Code { get; set; } = string.Empty;
    public string DiagnosisName { get; set; } = string.Empty;
}

// ── Write ─────────────────────────────────────────────────────────────────────
public class CreateDiagnosisDto
{
    public string ICD10Code { get; set; } = string.Empty;
    public string DiagnosisName { get; set; } = string.Empty;
}

public class AssignDiagnosisToVisitDto
{
    public string ICD10Code { get; set; } = string.Empty;
}