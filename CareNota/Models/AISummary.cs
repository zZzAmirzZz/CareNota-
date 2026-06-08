using System.ComponentModel.DataAnnotations;

namespace CareNota.Models;

public class AISummary
{
    [Key]
    public int AISummaryID { get; set; }

    public string SummaryText { get; set; } = string.Empty;
    // SummaryType = "Doctor"  → { subjective, objective, assessment, plan }
    // SummaryType = "Patient" → { diagnosis, symptoms, treatment_plan, when_to_seek_help, follow_up }
    public string SummaryType { get; set; } = string.Empty;

    // doctor rates after reviewing (1–5), used for model improvement
    public int? DoctorRating { get; set; }
    public string? DoctorFeedback { get; set; }

    // FK
    public int VisitID { get; set; }

    // Navigation
    public Visit Visit { get; set; } = null!;
}