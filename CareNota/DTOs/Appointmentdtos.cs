
using CareNota.Models;

namespace CareNota.DTOs.Appointment;

// ── Read ──────────────────────────────────────────────────────────────────────

public class AppointmentDto
{
    public int AppointmentID { get; set; }

    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public AppointmentStatus Status { get; set; }
    public string AppointmentType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int PatientID { get; set; }
    public string PatientName { get; set; } = string.Empty;  // From Patient.User
    public int DoctorID { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public int ReceptionistID { get; set; }
}

public class AppointmentDetailDto : AppointmentDto
{
    public VisitSummaryDto? Visit { get; set; }
}

public class VisitSummaryDto
{
    public int VisitID { get; set; }
    public DateTime VisitDate { get; set; }
    public string Assessment { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
}

// ── Write ─────────────────────────────────────────────────────────────────────

public class CreateAppointmentDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public string AppointmentType { get; set; } = string.Empty;

    public int PatientID { get; set; }
    public int DoctorID { get; set; }
    public int ReceptionistID { get; set; }
}

public class UpdateAppointmentDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }

    public AppointmentStatus Status { get; set; }
    public string AppointmentType { get; set; } = string.Empty;
}