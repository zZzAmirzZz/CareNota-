using CareNota.Models;

namespace CareNota.Repositories.Interfaces;

// ══════════════════════════════════════════════════════════════════════════════
// IAudioRepository
// ══════════════════════════════════════════════════════════════════════════════

public interface IAudioRepository
{
    Task<AudioRecord?> GetByIdAsync(int audioId);
    Task<AudioRecord?> GetByVisitIdAsync(int visitId);

    /// <summary>Returns all records whose DeletionAt is in the past and the blob has not been deleted yet.</summary>
    Task<List<AudioRecord>> GetPendingDeletionsAsync();

    Task AddAsync(AudioRecord audioRecord);
    void Delete(AudioRecord audioRecord);
    Task SaveAsync();
}


