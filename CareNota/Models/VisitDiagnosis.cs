namespace CareNota.Models;

public class VisitDiagnosis
{
    // Composite PK (configured in DbContext)
    public int VisitID { get; set; }
    public string ICD10Code { get; set; } = string.Empty;

    // Navigation
    public Visit Visit { get; set; } = null!;
    public Diagnosis Diagnosis { get; set; } = null!;
}