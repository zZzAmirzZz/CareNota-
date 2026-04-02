namespace CareNota.Models;

public class Doctor
{
    public string DoctorName { get; set; } = string.Empty;
    public int DoctorID { get; set; }
    public string Specialty { get; set; } = string.Empty;

    // FK → ApplicationUser (string GUID)
    public string UserId { get; set; } = string.Empty;

    // Navigation
    public ApplicationUser User { get; set; } = null!;
}