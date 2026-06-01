namespace CareNota.DTOs.Diagnosis;

public class DiagnosisDto
{
    public int DiagnosisID { get; set; }
    public string DiagnosisName { get; set; } = string.Empty;
    public int VisitID { get; set; }
}

public class CreateDiagnosisDto
{
    public string DiagnosisName { get; set; } = string.Empty;
    public int VisitID { get; set; }
}