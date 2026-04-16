using CareNota.Models;

public interface IAppointmentRepository : IRepository<Appointment>
{
    // Single appointment including its Visit navigation
    Task<Appointment?> GetWithVisitAsync(int AppointmentId);

    // Full graph: Patient + Receptionist + Visit + LabTests + Reminders
    Task<Appointment?> GetFullDetailsAsync(int AppointmentId);

    // All appointments for one patient, ordered by date desc
    Task<IEnumerable<Appointment>> GetByPatientIdAsync(int PatientId);

    // All appointments managed by one receptionist
    Task<IEnumerable<Appointment>> GetByReceptionistIdAsync(int ReceptionistId);

    // Filter by status string ("Scheduled", "Completed", "Cancelled" …)
    Task<IEnumerable<Appointment>> GetByStatusAsync(string Status);

    // Date range query (inclusive, ordered by date asc)
    Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime From, DateTime To);

    // Returns true if the patient already has a non-cancelled appointment on that day
    Task<bool> HasConflictAsync(int PatientId, DateTime AppointmentDate);
}