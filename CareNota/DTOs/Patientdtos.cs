namespace CareNota.DTOs.Patient;

// ── Read ──────────────────────────────────────────────────────────────────────

public class PatientDto
{
    public int PatientID { get; set; }
    public string FullName { get; set; } = string.Empty;  // FirstName + LastName from ApplicationUser
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string BloodType { get; set; } = string.Empty;
    public string Allergies { get; set; } = string.Empty;
    public string InsuranceInfo { get; set; } = string.Empty;
    public int Age { get; set; }                  // Calculated from DateOfBirth
}

public class PatientDetailDto : PatientDto
{
    public MedicalHistorySummaryDto? MedicalHistory { get; set; }
    public IList<AppointmentSummaryDto> Appointments { get; set; } = [];
}

public class MedicalHistorySummaryDto
{
    public string ChiefComplaint { get; set; } = string.Empty;
    public string PresentIllness { get; set; } = string.Empty;
    public string PastMedicalHistory { get; set; } = string.Empty;
}

public class AppointmentSummaryDto
{
    public int AppointmentID { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string AppointmentType { get; set; } = string.Empty;
}

// ── Write ─────────────────────────────────────────────────────────────────────

public class UpdatePatientDto
{
    public string Gender { get; set; } = string.Empty;
    public string BloodType { get; set; } = string.Empty;
    public string Allergies { get; set; } = string.Empty;
    public string InsuranceInfo { get; set; } = string.Empty;
}