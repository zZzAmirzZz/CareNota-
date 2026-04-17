using CareNota.Data;
using CareNota.Models;
using CareNota.Repositories;
using Microsoft.EntityFrameworkCore;

public class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    public PatientRepository(ApplicationDbContext Context) : base(Context) { }

    public async Task<Patient?> GetByUserIdAsync(string UserId)
        => await DbSet
            .Include(P => P.User)
            .FirstOrDefaultAsync(P => P.UserId == UserId);

    public async Task<Patient?> GetWithMedicalHistoryAsync(int PatientId)
        => await DbSet
            .Include(P => P.User)
            .Include(P => P.MedicalHistory)
            .FirstOrDefaultAsync(P => P.PatientID == PatientId);

    public async Task<Patient?> GetWithAppointmentsAsync(int PatientId)
        => await DbSet
            .Include(P => P.User)
            .Include(P => P.Appointments)
            .FirstOrDefaultAsync(P => P.PatientID == PatientId);

    public async Task<Patient?> GetWithRemindersAsync(int PatientId)
        => await DbSet
            .Include(P => P.User)
            .Include(P => P.Reminders)
            .FirstOrDefaultAsync(P => P.PatientID == PatientId);

    public async Task<IEnumerable<Patient>> SearchByNameAsync(string Name)
        => await DbSet
            .Include(P => P.User)
            .Where(P => (P.User.FullName)
                .ToLower().Contains(Name.ToLower()))
            .AsNoTracking()
            .ToListAsync();
}
