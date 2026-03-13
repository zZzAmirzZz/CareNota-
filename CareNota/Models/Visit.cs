namespace CareNota.Models
{
    public class Visit
    {
        public int VisitID { get; set; }
        public DateTime VisitDate { get; set; }
        public string Subjective { get; set; }
        public string Objective { get; set; }
        public string Assessment { get; set; }
        public string Plan { get; set; }

        public int AppointmentID { get; set; }
    }
}
