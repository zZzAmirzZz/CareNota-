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
    public async Task<AISummary?> GetLastApprovedDoctorSummaryByPatientAsync(
    int PatientId, int ExcludeVisitId)
    => await _Context.AISummaries
        .Where(S =>
            S.SummaryType == "Doctor" &&
            S.VisitID != ExcludeVisitId &&
            S.Visit.Appointment.PatientID == PatientId &&
            S.Visit.Subjective != null)   // Subjective != null = approved
        .OrderByDescending(S => S.Visit.VisitDate)
        .FirstOrDefaultAsync();
}