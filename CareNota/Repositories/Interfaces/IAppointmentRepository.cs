using CareNota.Models;
using static CareNota.Models.Appointment;

public interface IAppointmentRepository : IRepository<Appointment>
{
    // ── Single / Details ─────────────────────────────
    Task<Appointment?> GetWithVisitAsync(int appointmentId);
    Task<Appointment?> GetFullDetailsAsync(int appointmentId);

    // ── Filters ──────────────────────────────────────
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
    Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId);
    Task<IEnumerable<Appointment>> GetByReceptionistIdAsync(int receptionistId);
    Task<IEnumerable<Appointment>> GetByStatusAsync(AppointmentStatus status);
    // ── Time-based Queries ───────────────────────────
    Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime from, DateTime to);

    // Weekly schedule
    Task<IEnumerable<Appointment>> GetDoctorWeeklyScheduleAsync(int doctorId, DateTime startOfWeek);

    // Daily appointments (used for slots)
    Task<IEnumerable<Appointment>> GetDoctorAppointmentsByDateAsync(int doctorId, DateTime date);

    // ── Conflict Checks ──────────────────────────────
    Task<bool> HasDoctorConflictAsync(int doctorId, DateTime start, DateTime end, int? excludeAppointmentId = null);
    Task<bool> HasPatientConflictAsync(int patientId, DateTime start, DateTime end, int? excludeAppointmentId = null);

    IQueryable<Appointment> QueryWithNames();
}
