using CareNota.Data;
using CareNota.Models;
using Microsoft.EntityFrameworkCore;

namespace CareNota.Repositories;

public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(ApplicationDbContext context) : base(context) { }

    // ── CENTRALIZED INCLUDES (FIXED) ─────────────────────────────
    private IQueryable<Appointment> GetAppointmentsWithNames()
    {
        return DbSet
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Include(a => a.Receptionist).ThenInclude(r => r.User);
    }

    // ── DETAILS ───────────────────────────────────────────────────

    public async Task<Appointment?> GetWithVisitAsync(int appointmentId)
        => await DbSet
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Include(a => a.Doctor).ThenInclude(d => d.User)   // ✅ FIXED
            .Include(a => a.Receptionist).ThenInclude(r => r.User)
            .Include(a => a.Visit)
            .FirstOrDefaultAsync(a => a.AppointmentID == appointmentId);

    public async Task<Appointment?> GetFullDetailsAsync(int appointmentId)
        => await DbSet
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Include(a => a.Receptionist).ThenInclude(r => r.User)
            .Include(a => a.Visit).ThenInclude(v => v.Prescription)
            .Include(a => a.Visit).ThenInclude(v => v.LabTests)
            .Include(a => a.Visit).ThenInclude(v => v.AISummaries)
            .Include(a => a.Reminders)
            .FirstOrDefaultAsync(a => a.AppointmentID == appointmentId);

    // ── FILTERS ───────────────────────────────────────────────────

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId)
        => await GetAppointmentsWithNames()
            .Where(a => a.PatientID == patientId)
            .OrderByDescending(a => a.StartTime)
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<Appointment>> GetByDoctorIdAsync(int doctorId)
        => await GetAppointmentsWithNames()
            .Where(a => a.DoctorID == doctorId)
            .OrderByDescending(a => a.StartTime)
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<Appointment>> GetByReceptionistIdAsync(int receptionistId)
        => await GetAppointmentsWithNames()
            .Where(a => a.ReceptionistID == receptionistId)
            .OrderByDescending(a => a.StartTime)
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<Appointment>> GetByStatusAsync(AppointmentStatus status)
        => await GetAppointmentsWithNames()
            .Where(a => a.Status == status)
            .OrderByDescending(a => a.StartTime)
            .AsNoTracking()
            .ToListAsync();

    // ── TIME BASED ───────────────────────────────────────────────

    public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(DateTime from, DateTime to)
        => await GetAppointmentsWithNames()
            .Where(a => a.StartTime < to && a.EndTime > from)
            .OrderBy(a => a.StartTime)
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<Appointment>> GetDoctorWeeklyScheduleAsync(int doctorId, DateTime startOfWeek)
    {
        var endOfWeek = startOfWeek.AddDays(7);

        return await GetAppointmentsWithNames()
            .Where(a => a.DoctorID == doctorId &&
                        a.StartTime < endOfWeek &&
                        a.EndTime > startOfWeek)
            .OrderBy(a => a.StartTime)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetDoctorAppointmentsByDateAsync(int doctorId, DateTime date)
    {
        var start = date.Date;
        var end = start.AddDays(1);

        return await GetAppointmentsWithNames()
            .Where(a => a.DoctorID == doctorId &&
                        a.StartTime < end &&
                        a.EndTime > start &&
                        a.Status != AppointmentStatus.Cancelled)
            .OrderBy(a => a.StartTime)
            .AsNoTracking()
            .ToListAsync();
    }

    // ── CONFLICT LOGIC ───────────────────────────────────────────

    public async Task<bool> HasDoctorConflictAsync(int doctorId, DateTime start, DateTime end, int? excludeAppointmentId = null)
    {
        return await DbSet.AnyAsync(a =>
            a.DoctorID == doctorId &&
            a.Status != AppointmentStatus.Cancelled &&
            (excludeAppointmentId == null || a.AppointmentID != excludeAppointmentId) &&
            start < a.EndTime &&
            end > a.StartTime);
    }

    public async Task<bool> HasPatientConflictAsync(int patientId, DateTime start, DateTime end, int? excludeAppointmentId = null)
    {
        return await DbSet.AnyAsync(a =>
            a.PatientID == patientId &&
            a.Status != AppointmentStatus.Cancelled &&
            (excludeAppointmentId == null || a.AppointmentID != excludeAppointmentId) &&
            start < a.EndTime &&
            end > a.StartTime);
    }


    public IQueryable<Appointment> QueryWithNames()
    {
        return DbSet
            .Include(a => a.Patient).ThenInclude(p => p.User)
            .Include(a => a.Doctor).ThenInclude(d => d.User)
            .Include(a => a.Receptionist).ThenInclude(r => r.User);
    }
}