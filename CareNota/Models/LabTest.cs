namespace CareNota.Models
{
    public class LabTest
    {
        public int LabTestID { get; set; }
        public string LabTestName { get; set; }
        public string TestResultURL { get; set; }

        public int VisitID { get; set; } // Foreign key to Visit
    }
}
