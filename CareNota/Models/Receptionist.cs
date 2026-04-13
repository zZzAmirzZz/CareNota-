using System.ComponentModel.DataAnnotations;

namespace CareNota.Models;

public class Receptionist
{

    [Key]
    public int ReceptionistID { get; set; }

    // FK → ApplicationUser (string GUID)
    public string UserId { get; set; } = string.Empty;

    // Navigation
    public ApplicationUser User { get; set; } = null!;
    public ICollection<Appointment> Appointments { get; set; } = [];
}