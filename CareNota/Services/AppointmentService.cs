using AutoMapper;
using CareNota.DTOs.Appointment;
using CareNota.Models;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _AppointmentRepo;
    private readonly IPatientRepository _PatientRepo;
    private readonly IMapper _Mapper;

    public AppointmentService(
        IAppointmentRepository AppointmentRepo,
        IPatientRepository PatientRepo,
        IMapper Mapper)
    {
        _AppointmentRepo = AppointmentRepo;
        _PatientRepo = PatientRepo;
        _Mapper = Mapper;
    }

    // ── Read ──────────────────────────────────────────────────────────────────

    public async Task<IEnumerable<AppointmentDto>> GetAllAsync()
    {
        var Appointments = await _AppointmentRepo.GetAllAsync();
        return _Mapper.Map<IEnumerable<AppointmentDto>>(Appointments);
    }

    public async Task<AppointmentDto?> GetByIdAsync(int AppointmentId)
    {
        var Appointment = await _AppointmentRepo.GetByIdAsync(AppointmentId);
        return Appointment is null ? null : _Mapper.Map<AppointmentDto>(Appointment);
    }

    public async Task<AppointmentDetailDto?> GetDetailsAsync(int AppointmentId)
    {
        var Appointment = await _AppointmentRepo.GetFullDetailsAsync(AppointmentId);
        return Appointment is null ? null : _Mapper.Map<AppointmentDetailDto>(Appointment);
    }

    public async Task<IEnumerable<AppointmentDto>> GetByPatientIdAsync(int PatientId)
    {
        var Appointments = await _AppointmentRepo.GetByPatientIdAsync(PatientId);
        return _Mapper.Map<IEnumerable<AppointmentDto>>(Appointments);
    }

    public async Task<IEnumerable<AppointmentDto>> GetByDateRangeAsync(
        DateTime From, DateTime To)
    {
        if (From > To)
            throw new ArgumentException("'From' date must be earlier than 'To' date.");

        var Appointments = await _AppointmentRepo.GetByDateRangeAsync(From, To);
        return _Mapper.Map<IEnumerable<AppointmentDto>>(Appointments);
    }

    public async Task<IEnumerable<AppointmentDto>> GetByStatusAsync(string Status)
    {
        var Appointments = await _AppointmentRepo.GetByStatusAsync(Status);
        return _Mapper.Map<IEnumerable<AppointmentDto>>(Appointments);
    }

    // ── Write ─────────────────────────────────────────────────────────────────

    public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto Dto)
    {
        // 1. Patient must exist
        var PatientExists = await _PatientRepo
            .ExistsAsync(P => P.PatientID == Dto.PatientID);
        if (!PatientExists)
            throw new KeyNotFoundException($"Patient {Dto.PatientID} not found.");

        // 2. No same-day conflict for this patient
        var HasConflict = await _AppointmentRepo
            .HasConflictAsync(Dto.PatientID, Dto.AppointmentDate);
        if (HasConflict)
            throw new InvalidOperationException(
                "Patient already has an active appointment on this date.");

        // 3. Map and persist — AutoMapper sets Status = "Scheduled", CreatedAt = UtcNow
        var newAppointment = _Mapper.Map<Appointment>(Dto);
        await _AppointmentRepo.AddAsync(newAppointment);
        await _AppointmentRepo.SaveChangesAsync();

        // 4. Re-fetch with Patient.User so PatientName is populated in the response
        var Created = await _AppointmentRepo.GetWithVisitAsync(newAppointment.AppointmentID);
        return _Mapper.Map<AppointmentDto>(Created!);
    }

    public async Task<AppointmentDto> UpdateAsync(
        int AppointmentId, UpdateAppointmentDto Dto)
    {
        var Appointment = await _AppointmentRepo.GetByIdAsync(AppointmentId)
            ?? throw new KeyNotFoundException($"Appointment {AppointmentId} not found.");

        if (Appointment.Status == "Cancelled")
            throw new InvalidOperationException(
                "Cannot update a cancelled appointment.");

        _Mapper.Map(Dto, Appointment);
        _AppointmentRepo.Update(Appointment);
        await _AppointmentRepo.SaveChangesAsync();

        var Updated = await _AppointmentRepo.GetWithVisitAsync(AppointmentId);
        return _Mapper.Map<AppointmentDto>(Updated!);
    }

    public async Task CancelAsync(int AppointmentId)
    {
        var Appointment = await _AppointmentRepo.GetByIdAsync(AppointmentId)
            ?? throw new KeyNotFoundException($"Appointment {AppointmentId} not found.");

        if (Appointment.Status == "Cancelled")
            throw new InvalidOperationException("Appointment is already cancelled.");

        Appointment.Status = "Cancelled";
        _AppointmentRepo.Update(Appointment);
        await _AppointmentRepo.SaveChangesAsync();
    }

    public async Task DeleteAsync(int AppointmentId)
    {
        var Appointment = await _AppointmentRepo.GetByIdAsync(AppointmentId)
            ?? throw new KeyNotFoundException($"Appointment {AppointmentId} not found.");

        _AppointmentRepo.Remove(Appointment);
        await _AppointmentRepo.SaveChangesAsync();
    }
}