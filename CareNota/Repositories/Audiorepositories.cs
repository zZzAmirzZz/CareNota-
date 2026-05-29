using CareNota.Data;
using CareNota.Models;
using CareNota.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CareNota.Repositories;

public class AudioRepository : IAudioRepository
{
    private readonly ApplicationDbContext _context;

    public AudioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<AudioRecord?> GetByIdAsync(int audioId)
        => await _context.AudioRecords.FindAsync(audioId);

    public async Task<AudioRecord?> GetByVisitIdAsync(int visitId)
        => await _context.AudioRecords
            .FirstOrDefaultAsync(a => a.VisitID == visitId);

    public async Task<List<AudioRecord>> GetPendingDeletionsAsync()
        => await _context.AudioRecords
            .Where(a => a.DeletionAt <= DateTime.UtcNow)
            .ToListAsync();

    public async Task AddAsync(AudioRecord audioRecord)
        => await _context.AudioRecords.AddAsync(audioRecord);

    public void Delete(AudioRecord audioRecord)
        => _context.AudioRecords.Remove(audioRecord);

    public async Task SaveAsync()
        => await _context.SaveChangesAsync();
}