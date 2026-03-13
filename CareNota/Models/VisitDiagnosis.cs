namespace CareNota.Models
{
    public class VisitDiagnosis
    {
        public int VisitID { get; set; } // Composite Key
        public string ICD10Code { get; set; } // Composite Key
    }
}
