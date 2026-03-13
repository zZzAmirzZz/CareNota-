namespace CareNota.Models
{
    public class AISummary
    {
        public int AISummaryID { get; set; }
        public string SummaryText { get; set; }
        public string SummaryType { get; set; }

        public float DoctorRating { get; set; }

        public int VisitID { get; set; } // Foreign key to Visit
    }
}
