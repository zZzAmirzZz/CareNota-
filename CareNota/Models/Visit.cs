using System.ComponentModel.DataAnnotations;

namespace CareNota.Models;

public class Visit
{
    [Key]
    public int VisitID { get; set; }

    public DateTime VisitDate { get; set; }

    // SOAP — null on AI path until approval, filled on manual path
    public string? Subjective { get; set; }
    public string? Objective { get; set; }
    public string? Assessment { get; set; }
    public string? Plan { get; set; }

    // Written on approval (AI path) or manually via PUT /visit/{id}
    public string? Symptoms { get; set; }        // Arabic, from patient_summary
    public string? WhenToSeekHelp { get; set; }  // Arabic, from patient_summary
    public string? FollowUp { get; set; } 

    // FK
    public int AppointmentID { get; set; }

    // Navigation
    public Appointment Appointment { get; set; } = null!;
    public Prescription? Prescription { get; set; }
    public AudioRecord? AudioRecord { get; set; }
    public ICollection<LabTest> LabTests { get; set; } = [];
    public ICollection<AISummary> AISummaries { get; set; } = [];
    public ICollection<Diagnosis> Diagnoses { get; set; } = []; // replaced VisitDiagnoses
}