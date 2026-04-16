using CareNota.DTOs.Appointment;

public interface IAppointmentService
{
    Task<IEnumerable<AppointmentDto>> GetAllAsync();

    Task<AppointmentDto?> GetByIdAsync(int AppointmentId);

    // Full graph including Visit summary
    Task<AppointmentDetailDto?> GetDetailsAsync(int AppointmentId);

    Task<IEnumerable<AppointmentDto>> GetByPatientIdAsync(int PatientId);

    Task<IEnumerable<AppointmentDto>> GetByDateRangeAsync(DateTime From, DateTime To);

    Task<IEnumerable<AppointmentDto>> GetByStatusAsync(string Status);

    // Creates with Status = "Scheduled"; validates patient exists + no same-day conflict
    Task<AppointmentDto> CreateAsync(CreateAppointmentDto Dto);

    // Can reschedule date/type and change status
    Task<AppointmentDto> UpdateAsync(int AppointmentId, UpdateAppointmentDto Dto);

    // Sets Status = "Cancelled" without deleting the row
    Task CancelAsync(int AppointmentId);

    Task DeleteAsync(int AppointmentId);
}