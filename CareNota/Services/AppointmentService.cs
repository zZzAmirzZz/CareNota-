using AutoMapper;
using CareNota.DTOs;
using CareNota.DTOs.Appointment;
using CareNota.Models;
using Microsoft.EntityFrameworkCore;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IPatientRepository _patientRepo;
    private readonly IMapper _mapper;

    public AppointmentService(
        IAppointmentRepository appointmentRepo,
        IPatientRepository patientRepo,
        IMapper mapper)
    {
        _appointmentRepo = appointmentRepo;
        _patientRepo = patientRepo;
        _mapper = mapper;
    }

    // ── READ ─────────────────────────────────────────

    public async Task<IEnumerable<AppointmentDto>> GetAllAsync()
    {
        var data = await _appointmentRepo
            .QueryWithNames()
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<AppointmentDto>>(data);
    }

    public async Task<AppointmentDto?> GetByIdAsync(int appointmentId)
    {
        var data = await _appointmentRepo
            .QueryWithNames()
            .FirstOrDefaultAsync(a => a.AppointmentID == appointmentId);
        return data == null ? null : _mapper.Map<AppointmentDto>(data);
    }

    public async Task<AppointmentDetailDto?> GetDetailsAsync(int appointmentId)
    {
        var data = await _appointmentRepo.GetFullDetailsAsync(appointmentId);
        return data == null ? null : _mapper.Map<AppointmentDetailDto>(data);
    }

    public async Task<IEnumerable<AppointmentDto>> GetByPatientIdAsync(int patientId)
    {
        var data = await _appointmentRepo.GetByPatientIdAsync(patientId);
        return _mapper.Map<IEnumerable<AppointmentDto>>(data);
    }

    public async Task<IEnumerable<AppointmentDto>> GetByDoctorIdAsync(int doctorId)
    {
        var data = await _appointmentRepo.GetByDoctorIdAsync(doctorId);
        return _mapper.Map<IEnumerable<AppointmentDto>>(data);
    }

    public async Task<IEnumerable<AppointmentDto>> GetByStatusAsync(AppointmentStatus status)
    {
        var data = await _appointmentRepo.GetByStatusAsync(status);
        return _mapper.Map<IEnumerable<AppointmentDto>>(data);
    }

    public async Task<IEnumerable<AppointmentDto>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        if (from >= to)
            throw new ArgumentException("Invalid date range.");

        var data = await _appointmentRepo.GetByDateRangeAsync(from, to);
        return _mapper.Map<IEnumerable<AppointmentDto>>(data);
    }

    public async Task<IEnumerable<AppointmentDto>> GetDoctorWeeklyScheduleAsync(int doctorId, DateTime startOfWeek)
    {
        var data = await _appointmentRepo.GetDoctorWeeklyScheduleAsync(doctorId, startOfWeek);
        return _mapper.Map<IEnumerable<AppointmentDto>>(data);
    }

    // ── AVAILABLE SLOTS ─────────────────────────────

    public async Task<IEnumerable<TimeSlotDto>> GetAvailableSlotsAsync(int doctorId, DateTime date)
    {
        var workStart = date.Date.AddHours(9);
        var workEnd = date.Date.AddHours(17);
        var slotDuration = TimeSpan.FromMinutes(30);

        var appointments = await _appointmentRepo
            .GetDoctorAppointmentsByDateAsync(doctorId, date);

        var availableSlots = new List<TimeSlotDto>();

        for (var time = workStart; time < workEnd; time += slotDuration)
        {
            var slotStart = time;
            var slotEnd = time + slotDuration;

            var hasConflict = appointments.Any(a =>
                slotStart < a.EndTime &&
                slotEnd > a.StartTime);

            if (!hasConflict)
            {
                availableSlots.Add(new TimeSlotDto
                {
                    Start = slotStart,
                    End = slotEnd
                });
            }
        }

        return availableSlots;
    }

    // ── CREATE ──────────────────────────────────────

    public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto)
    {
        if (dto.StartTime >= dto.EndTime)
            throw new ArgumentException("StartTime must be before EndTime.");

        var patientExists = await _patientRepo
            .ExistsAsync(p => p.PatientID == dto.PatientID);

        if (!patientExists)
            throw new KeyNotFoundException("Patient not found.");

        var doctorConflict = await _appointmentRepo
            .HasDoctorConflictAsync(dto.DoctorID, dto.StartTime, dto.EndTime);

        if (doctorConflict)
            throw new InvalidOperationException("Doctor is not available at this time.");

        var patientConflict = await _appointmentRepo
            .HasPatientConflictAsync(dto.PatientID, dto.StartTime, dto.EndTime);

        if (patientConflict)
            throw new InvalidOperationException("Patient already has an overlapping appointment.");

        var appointment = _mapper.Map<Appointment>(dto);

        appointment.Status = AppointmentStatus.Scheduled; // ✅ FIX
        appointment.CreatedAt = DateTime.UtcNow;

        await _appointmentRepo.AddAsync(appointment);
        await _appointmentRepo.SaveChangesAsync();

        var created = await _appointmentRepo.GetWithVisitAsync(appointment.AppointmentID);
        return _mapper.Map<AppointmentDto>(created!);
    }

    // ── UPDATE ──────────────────────────────────────

    public async Task<AppointmentDto> UpdateAsync(int appointmentId, UpdateAppointmentDto dto)
    {
        var appointment = await _appointmentRepo.GetByIdAsync(appointmentId)
            ?? throw new KeyNotFoundException("Appointment not found.");

        if (appointment.Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Cannot update cancelled appointment.");

        if (dto.StartTime >= dto.EndTime)
            throw new ArgumentException("Invalid time range.");

        var doctorConflict = await _appointmentRepo
            .HasDoctorConflictAsync(appointment.DoctorID, dto.StartTime, dto.EndTime, appointmentId);

        if (doctorConflict)
            throw new InvalidOperationException("Doctor not available.");

        _mapper.Map(dto, appointment);

        _appointmentRepo.Update(appointment);
        await _appointmentRepo.SaveChangesAsync();

        var updated = await _appointmentRepo.GetWithVisitAsync(appointmentId);
        return _mapper.Map<AppointmentDto>(updated!);
    }

    // ── CANCEL ──────────────────────────────────────

    public async Task CancelAsync(int appointmentId)
    {
        var appointment = await _appointmentRepo.GetByIdAsync(appointmentId)
            ?? throw new KeyNotFoundException("Appointment not found.");

        if (appointment.Status == AppointmentStatus.Cancelled)
            throw new InvalidOperationException("Already cancelled.");

        appointment.Status = AppointmentStatus.Cancelled;

        _appointmentRepo.Update(appointment);
        await _appointmentRepo.SaveChangesAsync();
    }

    // ── DELETE ──────────────────────────────────────

    public async Task DeleteAsync(int appointmentId)
    {
        var appointment = await _appointmentRepo.GetByIdAsync(appointmentId)
            ?? throw new KeyNotFoundException("Appointment not found.");

        _appointmentRepo.Remove(appointment);
        await _appointmentRepo.SaveChangesAsync();
    }
}