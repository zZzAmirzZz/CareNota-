using System.ComponentModel.DataAnnotations;

namespace CareNota.Models;

public class Visit
{
    [Key]
    public int VisitID { get; set; }
    public DateTime VisitDate { get; set; }
    public string Subjective { get; set; } = string.Empty;
    public string Objective { get; set; } = string.Empty;
    public string Assessment { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;

    // FK
    public int AppointmentID { get; set; }

    // Navigation
    public Appointment Appointment { get; set; } = null!;
    public Prescription? Prescription { get; set; }
    public AudioRecord? AudioRecord { get; set; }
    public ICollection<LabTest> LabTests { get; set; } = [];
    public ICollection<AISummary> AISummaries { get; set; } = [];
    public ICollection<VisitDiagnosis> VisitDiagnoses { get; set; } = [];
}