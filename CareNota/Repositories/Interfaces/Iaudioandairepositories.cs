using CareNota.Models;

namespace CareNota.Repositories.Interfaces;

// ══════════════════════════════════════════════════════════════════════════════
// IAudioRepository
// ══════════════════════════════════════════════════════════════════════════════
public interface IAudioRepository : IRepository<AudioRecord>
{
    Task<AudioRecord?> GetByVisitIdAsync(int VisitId);

    // Returns all AudioRecords whose DeletionAt <= now (ready to be deleted)
    Task<IEnumerable<AudioRecord>> GetExpiredRecordsAsync();
}

// ══════════════════════════════════════════════════════════════════════════════
// IAISummaryRepository
// ══════════════════════════════════════════════════════════════════════════════
//public interface IAISummaryRepository : IRepository<AISummary>
//{
//    Task<IEnumerable<AISummary>> GetByVisitIdAsync(int VisitId);

//    // Get the Doctor-facing summary for a visit
//    Task<AISummary?> GetDoctorSummaryAsync(int VisitId);

//    // Get the Patient-facing summary for a visit
//    Task<AISummary?> GetPatientSummaryAsync(int VisitId);
//}