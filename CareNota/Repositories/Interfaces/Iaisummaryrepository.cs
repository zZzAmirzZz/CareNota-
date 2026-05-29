using CareNota.Models;

namespace CareNota.Repositories.Interfaces;

public interface IAISummaryRepository
{
    Task<AISummary?> GetByVisitAndTypeAsync(int VisitId, string SummaryType);
    Task AddAsync(AISummary Summary);
    Task SaveAsync();
}
