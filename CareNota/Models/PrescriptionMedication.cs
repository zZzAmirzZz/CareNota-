namespace CareNota.Models
{
    public class PrescriptionMedication
    {
        public int PrescriptionID { get; set; }
        public int MedicationID { get; set; }
        public string Dosage { get; set; }
        public string Frequency { get; set; }
        public string Route { get; set; }
        public string Duration { get; set; }

        public string Notes { get; set; }
    }
}
