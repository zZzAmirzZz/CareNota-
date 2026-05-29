using CareNota.Data;
using CareNota.Models;
using CareNota.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using CareNota.DTOs.Summary;
namespace CareNota.Repositories;

public class AISummaryRepository : IAISummaryRepository
{
    private readonly ApplicationDbContext _Context;

    public AISummaryRepository(ApplicationDbContext Context)
    {
        _Context = Context;
    }

    public async Task<AISummary?> GetByVisitAndTypeAsync(int VisitId, string SummaryType)
        => await _Context.AISummaries
            .FirstOrDefaultAsync(s => s.VisitID == VisitId && s.SummaryType == SummaryType);

    public async Task AddAsync(AISummary Summary)
        => await _Context.AISummaries.AddAsync(Summary);

    public async Task SaveAsync()
        => await _Context.SaveChangesAsync();
}