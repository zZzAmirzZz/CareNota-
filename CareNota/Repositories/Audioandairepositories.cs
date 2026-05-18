//using CareNota.Data;
//using CareNota.Models;
//using CareNota.Repositories.Interfaces;
//using Microsoft.EntityFrameworkCore;

//namespace CareNota.Repositories;

//// ══════════════════════════════════════════════════════════════════════════════
//// AudioRepository
//// ══════════════════════════════════════════════════════════════════════════════
//public class AudioRepository : GenericRepository<AudioRecord>, IAudioRepository
//{
//    public AudioRepository(ApplicationDbContext Context) : base(Context) { }

//    public async Task<AudioRecord?> GetByVisitIdAsync(int VisitId)
//        => await DbSet.FirstOrDefaultAsync(A => A.VisitID == VisitId);

//    public async Task<IEnumerable<AudioRecord>> GetExpiredRecordsAsync()
//        => await DbSet
//            .Where(A => A.DeletionAt <= DateTime.UtcNow)
//            .ToListAsync();
//}

// ══════════════════════════════════════════════════════════════════════════════
// AISummaryRepository
// ══════════════════════════════════════════════════════════════════════════════
//public class AISummaryRepository : GenericRepository<AISummary>, IAISummaryRepository
//{
//    public AISummaryRepository(ApplicationDbContext Context) : base(Context) { }

//    public async Task<IEnumerable<AISummary>> GetByVisitIdAsync(int VisitId)
//        => await DbSet
//            .Where(S => S.VisitID == VisitId)
//            .AsNoTracking()
//            .ToListAsync();

//    public async Task<AISummary?> GetDoctorSummaryAsync(int VisitId)
//        => await DbSet.FirstOrDefaultAsync(S =>
//            S.VisitID == VisitId && S.SummaryType == "Doctor");

//    public async Task<AISummary?> GetPatientSummaryAsync(int VisitId)
//        => await DbSet.FirstOrDefaultAsync(S =>
//            S.VisitID == VisitId && S.SummaryType == "Patient");
//}