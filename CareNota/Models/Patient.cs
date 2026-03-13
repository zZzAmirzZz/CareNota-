namespace CareNota.Models
{
    public class Patient
    {
        public int PatientID { get; set; }
        public string Gender { get; set; }

        public DateTime DateOfBirth { get; set; }
        public string BloodType { get; set; }
        public string Allergies { get; set; }
        public string InsuranceInfo { get; set; }

        public int UserID { get; set; } // Foreign Key to UserAccount
    }
}
