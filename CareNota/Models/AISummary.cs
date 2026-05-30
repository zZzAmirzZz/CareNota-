using System.ComponentModel.DataAnnotations;

namespace CareNota.Models;

public class AISummary
{
    [Key]
    public int AISummaryID { get; set; }
    public string SummaryText { get; set; } = string.Empty;
    // Stores the nested AI object serialised as JSON.
    // SummaryType = "Doctor"  → { subjective, objective, assessment, plan }
    // SummaryType = "Patient" → { diagnosis, symptoms, treatmentPlan }
    public string SummaryType { get; set; } = string.Empty;

    // FK
    public int VisitID { get; set; }

    // Navigation
    public Visit Visit { get; set; } = null!;
}
