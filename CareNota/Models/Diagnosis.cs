using System.ComponentModel.DataAnnotations;

namespace CareNota.Models;

public class Diagnosis
{
    [Key]
    public int DiagnosisID { get; set; }

    [Required]
    public string DiagnosisName { get; set; } = string.Empty; // Arabic from AI or manual

    // FK
    public int VisitID { get; set; }

    // Navigation
    public Visit Visit { get; set; } = null!;
}