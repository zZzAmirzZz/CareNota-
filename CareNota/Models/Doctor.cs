namespace CareNota.Models
{
    public class Doctor
    {
        public int DoctorID { get; set; }
        public string Specialty { get; set; }

        public int UserID { get; set; } // Foreign Key to UserAccount
    }
}
