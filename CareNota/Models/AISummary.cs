namespace CareNota.Models;

public class AISummary
{
    public int AISummaryID { get; set; }
    public string SummaryText { get; set; } = string.Empty;
    public string SummaryType { get; set; } = string.Empty;
    public float DoctorRating { get; set; }

    // FK
    public int VisitID { get; set; }

    // Navigation
    public Visit Visit { get; set; } = null!;
}
