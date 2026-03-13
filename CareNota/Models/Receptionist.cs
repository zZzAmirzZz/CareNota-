namespace CareNota.Models
{
    public class Receptionist
    {
        public int ReceptionistID { get; set; }

        public int UserID { get; set; } // Foreign Key to UserAccount
    }
}
