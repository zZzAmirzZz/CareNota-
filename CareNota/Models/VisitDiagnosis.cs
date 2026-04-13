using System.ComponentModel.DataAnnotations;

namespace CareNota.Models;

public class VisitDiagnosis
{
    // Composite PK (configured in DbContext)
    [Key]
    public int VisitID { get; set; }
    public string ICD10Code { get; set; } = string.Empty;

    // Navigation
    public Visit Visit { get; set; } = null!;
    public Diagnosis Diagnosis { get; set; } = null!;
}