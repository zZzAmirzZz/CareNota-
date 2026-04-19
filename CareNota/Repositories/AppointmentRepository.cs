using CareNota.Data;
using CareNota.Models;
using CareNota.Repositories;
using Microsoft.EntityFrameworkCore;


public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
{
    public AppointmentRepository(ApplicationDbContext Context) : base(Context) { }

    public async Task<Appointment?> GetWithVisitAsync(int AppointmentId)
        => await DbSet
            .Include(A => A.Patient).ThenInclude(P => P.User)
            .Include(A => A.Visit)
            .FirstOrDefaultAsync(A => A.AppointmentID == AppointmentId);

    public async Task<Appointment?> GetFullDetailsAsync(int AppointmentId)
        => await DbSet
            .Include(A => A.Patient).ThenInclude(P => P.User)
            .Include(A => A.Receptionist).ThenInclude(R => R.User)
            .Include(A => A.Visit)
                .ThenInclude(V => V != null ? V.Prescription : null)
            .Include(A => A.Visit)
                .ThenInclude(V => V != null ? V.LabTests : null)
            .Include(A => A.Visit)
                .ThenInclude(V => V != null ? V.AISummaries : null)
            .Include(A => A.Reminders)
            .FirstOrDefaultAsync(A => A.AppointmentID == AppointmentId);

    public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int PatientId)
        => await DbSet
            .Include(A => A.Patient).ThenInclude(P => P.User)
            .Include(A => A.Visit)
            .Where(A => A.PatientID == PatientId)
            .OrderByDescending(A => A.AppointmentDate)
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<Appointment>> GetByReceptionistIdAsync(int ReceptionistId)
        => await DbSet
            .Include(A => A.Patient).ThenInclude(P => P.User)
            .Where(A => A.ReceptionistID == ReceptionistId)
            .OrderByDescending(A => A.AppointmentDate)
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<Appointment>> GetByStatusAsync(string Status)
        => await DbSet
            .Include(A => A.Patient).ThenInclude(P => P.User)
            .Where(A => A.Status == Status)
            .OrderByDescending(A => A.AppointmentDate)
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<Appointment>> GetByDateRangeAsync(
        DateTime From, DateTime To)
        => await DbSet
            .Include(A => A.Patient).ThenInclude(P => P.User)
            .Where(A => A.AppointmentDate >= From && A.AppointmentDate <= To)
            .OrderBy(A => A.AppointmentDate)
            .AsNoTracking()
            .ToListAsync();

    public async Task<bool> HasConflictAsync(int PatientId, DateTime AppointmentDate)
        => await DbSet.AnyAsync(A =>
            A.PatientID == PatientId &&
            A.AppointmentDate.Date == AppointmentDate.Date &&
            A.Status != "Cancelled");
}