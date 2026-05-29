using CareNota.DTOs.Summary;
using CareNota.Interfaces;
using CareNota.Models;
using CareNota.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using CareNota.Services.Interfaces;
using CareNota.DTOs.Summary;
namespace CareNota.Services;

public class SummaryService : ISummaryService
{
    private readonly IAISummaryRepository _AISummaryRepository;
    private readonly IVisitRepository _VisitRepository;   // your existing full repo
    private readonly ILogger<SummaryService> _Logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public SummaryService(
        IAISummaryRepository AISummaryRepository,
        IVisitRepository VisitRepository,
        ILogger<SummaryService> Logger)
    {
        _AISummaryRepository = AISummaryRepository;
        _VisitRepository = VisitRepository;
        _Logger = Logger;
    }

    // ── GET /api/visits/{id}/summary ─────────────────────────────────────────
    public async Task<VisitSummaryResponseDto> GetSummaryAsync(int VisitId)
    {
        var DoctorRecord = await _AISummaryRepository.GetByVisitAndTypeAsync(VisitId, "Doctor")
            ?? throw new KeyNotFoundException(
                $"AI summary not ready yet for VisitId {VisitId}. Try again in a moment.");

        var PatientRecord = await _AISummaryRepository.GetByVisitAndTypeAsync(VisitId, "Patient");

        var DoctorData = JsonSerializer.Deserialize<DoctorSummaryAIDto>(
            DoctorRecord.SummaryText, JsonOptions) ?? new DoctorSummaryAIDto();

        var PatientData = PatientRecord is not null
            ? JsonSerializer.Deserialize<PatientSummaryAIDto>(
                PatientRecord.SummaryText, JsonOptions) ?? new PatientSummaryAIDto()
            : new PatientSummaryAIDto();

        // IsApproved = SOAP has already been written to Visit
        var Visit = await _VisitRepository.GetByIdAsync(VisitId);
        var IsApproved = !string.IsNullOrWhiteSpace(Visit?.Subjective);

        return new VisitSummaryResponseDto
        {
            VisitId = VisitId,
            IsApproved = IsApproved,
            DoctorSummary = new DoctorSummaryDto
            {
                AISummaryId = DoctorRecord.AISummaryID,
                Subjective = DoctorData.Subjective,
                Objective = DoctorData.Objective,
                Assessment = DoctorData.Assessment,
                Plan = DoctorData.Plan
            },
            PatientSummary = new PatientSummaryDto
            {
                AISummaryId = PatientRecord?.AISummaryID ?? 0,
                Diagnosis = PatientData.Diagnosis,
                Symptoms = PatientData.Symptoms,
                TreatmentPlan = PatientData.TreatmentPlan,
                WhenToSeekHelp = PatientData.WhenToSeekHelp
            }
        };
    }

    // ── PUT /api/visits/{id}/summary ─────────────────────────────────────────
    public async Task EditSummaryAsync(int VisitId, EditSummaryDto Dto)
    {
        var DoctorRecord = await _AISummaryRepository.GetByVisitAndTypeAsync(VisitId, "Doctor")
            ?? throw new KeyNotFoundException($"No Doctor summary found for VisitId {VisitId}.");

        var DoctorData = JsonSerializer.Deserialize<DoctorSummaryAIDto>(
            DoctorRecord.SummaryText, JsonOptions) ?? new DoctorSummaryAIDto();

        if (Dto.Subjective is not null) DoctorData.Subjective = Dto.Subjective;
        if (Dto.Objective is not null) DoctorData.Objective = Dto.Objective;
        if (Dto.Assessment is not null) DoctorData.Assessment = Dto.Assessment;
        if (Dto.Plan is not null) DoctorData.Plan = Dto.Plan;

        DoctorRecord.SummaryText = JsonSerializer.Serialize(DoctorData);

        if (Dto.WhenToSeekHelp is not null)
        {
            var PatientRecord = await _AISummaryRepository.GetByVisitAndTypeAsync(VisitId, "Patient");
            if (PatientRecord is not null)
            {
                var PatientData = JsonSerializer.Deserialize<PatientSummaryAIDto>(
                    PatientRecord.SummaryText, JsonOptions) ?? new PatientSummaryAIDto();

                PatientData.WhenToSeekHelp = Dto.WhenToSeekHelp;
                PatientRecord.SummaryText = JsonSerializer.Serialize(PatientData);
            }
        }

        // ← SaveChangesAsync matches your existing VisitRepository/DbContext pattern
        await _AISummaryRepository.SaveAsync();

        _Logger.LogInformation("Doctor edited draft summary for VisitId {VisitId}.", VisitId);
    }

    // ── POST /api/visits/{id}/summary/approve ────────────────────────────────
    public async Task ApproveSummaryAsync(int VisitId, ApproveSummaryDto Dto)
    {
        var DoctorRecord = await _AISummaryRepository.GetByVisitAndTypeAsync(VisitId, "Doctor")
            ?? throw new KeyNotFoundException($"No Doctor summary found for VisitId {VisitId}.");

        var DoctorData = JsonSerializer.Deserialize<DoctorSummaryAIDto>(
            DoctorRecord.SummaryText, JsonOptions)
            ?? throw new InvalidOperationException("Doctor summary JSON is corrupt.");

        var PatientRecord = await _AISummaryRepository.GetByVisitAndTypeAsync(VisitId, "Patient");
        var PatientData = PatientRecord is not null
            ? JsonSerializer.Deserialize<PatientSummaryAIDto>(
                PatientRecord.SummaryText, JsonOptions) ?? new PatientSummaryAIDto()
            : new PatientSummaryAIDto();

        // Load Visit using YOUR existing repo method
        var Visit = await _VisitRepository.GetByIdAsync(VisitId)
            ?? throw new KeyNotFoundException($"Visit {VisitId} not found.");

        // Write SOAP into Visit (permanent clinical record)
        Visit.Subjective = DoctorData.Subjective;
        Visit.Objective = DoctorData.Objective;
        Visit.Assessment = DoctorData.Assessment;
        Visit.Plan = DoctorData.Plan;

        // Write patient fields into Visit
        Visit.WhenToSeekHelp = PatientData.WhenToSeekHelp;
        Visit.FollowUpDate = Dto.FollowUpDate;

        // ← your repo uses SaveChangesAsync()
        await _VisitRepository.SaveChangesAsync();

        _Logger.LogInformation(
            "Summary approved for VisitId {VisitId}. FollowUpDate={Date}",
            VisitId, Dto.FollowUpDate);
    }

    // ── GET /api/visits/{id}/patient-summary ─────────────────────────────────
    public async Task<PatientSummaryViewDto> GetPatientSummaryAsync(int VisitId)
    {
        var Visit = await _VisitRepository.GetByIdAsync(VisitId)
            ?? throw new KeyNotFoundException($"Visit {VisitId} not found.");

        if (string.IsNullOrWhiteSpace(Visit.Subjective))
            throw new InvalidOperationException(
                "Summary has not been approved by the doctor yet.");

        var PatientRecord = await _AISummaryRepository.GetByVisitAndTypeAsync(VisitId, "Patient")
            ?? throw new KeyNotFoundException($"No patient summary found for VisitId {VisitId}.");

        var PatientData = JsonSerializer.Deserialize<PatientSummaryAIDto>(
            PatientRecord.SummaryText, JsonOptions) ?? new PatientSummaryAIDto();

        return new PatientSummaryViewDto
        {
            VisitId = VisitId,
            VisitDate = Visit.VisitDate,
            Diagnosis = PatientData.Diagnosis,
            Symptoms = PatientData.Symptoms,
            TreatmentPlan = PatientData.TreatmentPlan,
            WhenToSeekHelp = Visit.WhenToSeekHelp ?? string.Empty,
            FollowUpDate = Visit.FollowUpDate
        };
    }
}

