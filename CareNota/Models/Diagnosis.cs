using System.ComponentModel.DataAnnotations;

namespace CareNota.Models;

public class Diagnosis
{
    // String PK (ICD-10 code e.g. "J18.9")
    [Key]
    public string ICD10Code { get; set; } = string.Empty;
    public string DiagnosisName { get; set; } = string.Empty;

    // Navigation
    public ICollection<VisitDiagnosis> VisitDiagnoses { get; set; } = [];
}