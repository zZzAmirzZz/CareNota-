namespace CareNota.DTOs.Doctor;

// ── Read ──────────────────────────────────────────────────────────────────────

public class DoctorDto
{
    public int DoctorID { get; set; }
    public string FullName { get; set; } = string.Empty;  // FirstName + LastName from ApplicationUser
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Specialty { get; set; } = string.Empty;
}

// ── Write ─────────────────────────────────────────────────────────────────────

public class UpdateDoctorDto
{
    public string Specialty { get; set; } = string.Empty;
}