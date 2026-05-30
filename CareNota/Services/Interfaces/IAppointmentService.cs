using CareNota.DTOs;
using CareNota.DTOs.Appointment;
using CareNota.Models;

namespace CareNota.Services.Interfaces
{
    public interface IAppointmentService
    {
        // Read
        Task<IEnumerable<AppointmentDto>> GetAllAsync();
        Task<AppointmentDto?> GetByIdAsync(int appointmentId);
        Task<AppointmentDetailDto?> GetDetailsAsync(int appointmentId);
        Task<IEnumerable<AppointmentDto>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<AppointmentDto>> GetByDoctorIdAsync(int doctorId);
        Task<IEnumerable<AppointmentDto>> GetByStatusAsync(AppointmentStatus status);
        Task<IEnumerable<AppointmentDto>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task<IEnumerable<AppointmentDto>> GetDoctorWeeklyScheduleAsync(int doctorId, DateTime startOfWeek);

        // Available Slots
        Task<IEnumerable<TimeSlotDto>> GetAvailableSlotsAsync(int doctorId, DateTime date);

        // Create
        Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto);

        // Update
        Task<AppointmentDto> UpdateAsync(int appointmentId, UpdateAppointmentDto dto);

        // Cancel
        Task CancelAsync(int appointmentId);

        // Delete
        Task DeleteAsync(int appointmentId);
    }
}