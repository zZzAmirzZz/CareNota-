using System.ComponentModel.DataAnnotations;

namespace CareNota.Models;

public class LabTest
{
    [Key]
    public int LabTestID { get; set; }
    public string LabTestName { get; set; } = string.Empty;
    public string TestResultURL { get; set; } = string.Empty;

    // FK
    public int VisitID { get; set; }

    // Navigation
    public Visit Visit { get; set; } = null!;
}