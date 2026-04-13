using System.ComponentModel.DataAnnotations;

namespace CareNota.Models;

public class Prescription
{

    [Key]
    public int PrescriptionID { get; set; }
    public string Instructions { get; set; } = string.Empty;

    // FK
    public int VisitID { get; set; }

    // Navigation
    public Visit Visit { get; set; } = null!;
    public ICollection<PrescriptionMedication> PrescriptionMedications { get; set; } = [];
    public ICollection<Reminder> Reminders { get; set; } = [];
}