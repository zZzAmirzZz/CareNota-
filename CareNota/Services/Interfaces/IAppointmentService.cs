using CareNota.DTOs.Appointment;
using CareNota.Models;


public interface IAppointmentService
{
    // ── Read ─────────────────────────────
    Task<IEnumerable<AppointmentDto>> GetAllAsync();
    Task<AppointmentDto?> GetByIdAsync(int appointmentId);
    Task<AppointmentDetailDto?> GetDetailsAsync(int appointmentId);

    Task<IEnumerable<AppointmentDto>> GetByPatientIdAsync(int patientId);
    Task<IEnumerable<AppointmentDto>> GetByDoctorIdAsync(int doctorId);

    Task<IEnumerable<AppointmentDto>> GetByStatusAsync(AppointmentStatus status);
    Task<IEnumerable<AppointmentDto>> GetByDateRangeAsync(DateTime from, DateTime to);

    // Weekly schedule
    Task<IEnumerable<AppointmentDto>> GetDoctorWeeklyScheduleAsync(int doctorId, DateTime startOfWeek);

    // 🔥 Available slots
    Task<IEnumerable<TimeSlotDto>> GetAvailableSlotsAsync(int doctorId, DateTime date);

    // ── Write ────────────────────────────
    Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto);
    Task<AppointmentDto> UpdateAsync(int appointmentId, UpdateAppointmentDto dto);

    Task CancelAsync(int appointmentId);
    Task DeleteAsync(int appointmentId);
}