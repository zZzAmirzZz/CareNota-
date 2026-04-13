namespace CareNota.DTOs.LabTest;

// ── Read ──────────────────────────────────────────────────────────────────────
public class LabTestDto
{
    public int LabTestID { get; set; }
    public string LabTestName { get; set; } = string.Empty;
    public string TestResultURL { get; set; } = string.Empty;
    public int VisitID { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public bool HasResult => !string.IsNullOrEmpty(TestResultURL);
}

// ── Write ─────────────────────────────────────────────────────────────────────
public class CreateLabTestDto
{
    public string LabTestName { get; set; } = string.Empty;
    public int VisitID { get; set; }
}

public class UploadLabResultDto
{
    public IFormFile ResultFile { get; set; } = null!;
}