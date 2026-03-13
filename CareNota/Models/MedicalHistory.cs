namespace CareNota.Models
{
    public class MedicalHistory
    {
        public int MedicalHistoryId { get; set; }

        public string ChiefComplaint { get; set; }

        public string presentIllness { get; set; }

        public string pastMedicalHistory { get; set; }

        public int PatientId { get; set; } // Foreign key to Patient


    }
}
