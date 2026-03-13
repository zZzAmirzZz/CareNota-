namespace CareNota.Models
{
    public class Prescription
    {
        public int PrescriptionID { get; set; }
        public DateTime Date { get; set; }

        public string Instructions { get; set; }

        public int VisitID { get; set; } // Foreign key to Visit
    }
}
