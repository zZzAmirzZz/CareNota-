using CareNota.Data;
using CareNota.Models;
using CareNota.Repositories;
using Microsoft.EntityFrameworkCore;

public class PatientRepository : GenericRepository<Patient>, IPatientRepository
{
    public PatientRepository(ApplicationDbContext Context) : base(Context) { }

    public async Task<Patient?> GetByUserIdAsync(string userId)
          => await DbSet
              .Include(p => p.User)
              .FirstOrDefaultAsync(p => p.UserId == userId);

    public async Task<Patient?> GetWithMedicalHistoryAsync(int patientId)
        => await DbSet
            .Include(p => p.User)
            .Include(p => p.MedicalHistory)
            .FirstOrDefaultAsync(p => p.PatientID == patientId);
    public async Task<Patient?> GetWithAppointmentsAsync(int patientId)
            => await DbSet
                .Include(p => p.User)
                .Include(p => p.Appointments)
                .FirstOrDefaultAsync(p => p.PatientID == patientId);
    public async Task<Patient?> GetWithRemindersAsync(int patientId)
           => await DbSet
               .Include(p => p.User)
               .Include(p => p.Reminders)
               .FirstOrDefaultAsync(p => p.PatientID == patientId);

    public async Task<IEnumerable<Patient>> SearchByNameAsync(string name)
          => await DbSet
              .Include(p => p.User)
              .Where(p => p.User.FullName.ToLower().Contains(name.ToLower()))
              .AsNoTracking()
              .ToListAsync();


    public  async Task<IEnumerable<Patient>> GetAllAsync()
        => await DbSet
            .Include(p => p.User)
            .AsNoTracking()
            .ToListAsync();

    // GetByIdAsync مع Include
    public  async Task<Patient?> GetByIdAsync(int patientId)
        => await DbSet
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.PatientID == patientId);
}
