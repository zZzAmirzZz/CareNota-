using CareNota.DTOs.Summary;
using CareNota.Models;
using CareNota.Repositories.Interfaces;
using CareNota.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using static CareNota.DTOs.Summary.EditSummaryDto;

namespace CareNota.Services;

public class SummaryService : ISummaryService
{
    private readonly IAISummaryRepository _AISummaryRepository;
    private readonly IVisitRepository _VisitRepository;
    private readonly IDiagnosisRepository _DiagnosisRepository;
    private readonly IPrescriptionRepository _PrescriptionRepository;
    private readonly ILogger<SummaryService> _Logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public SummaryService(
        IAISummaryRepository AISummaryRepository,
        IVisitRepository VisitRepository,
        IDiagnosisRepository DiagnosisRepository,
        IPrescriptionRepository PrescriptionRepository,
        ILogger<SummaryService> Logger)
    {
        _AISummaryRepository = AISummaryRepository;
        _VisitRepository = VisitRepository;
        _DiagnosisRepository = DiagnosisRepository;
        _PrescriptionRepository = PrescriptionRepository;
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
                Plan = DoctorData.Plan,

                  ComparisonWithPreviousVisit = DoctorData.ComparisonWithPreviousVisit
            },
            PatientSummary = new PatientSummaryDto
            {
                AISummaryId = PatientRecord?.AISummaryID ?? 0,
                Diagnosis = PatientData.Diagnosis,
                Symptoms = PatientData.Symptoms,
                TreatmentPlan = PatientData.TreatmentPlan,
                WhenToSeekHelp = PatientData.WhenToSeekHelp,
                   FollowUp = PatientData.FollowUp
            }
        };
    }

    // ── PUT /api/visits/{id}/summary ─────────────────────────────────────────
    // Only non-null fields are updated — existing values are preserved.
    public async Task EditSummaryAsync(int VisitId, EditSummaryDto Dto)
    {
        var DoctorRecord = await _AISummaryRepository.GetByVisitAndTypeAsync(VisitId, "Doctor")
            ?? throw new KeyNotFoundException($"No Doctor summary found for VisitId {VisitId}.");

        // ── Doctor-side (SOAP) ────────────────────────────────────────────────
        var DoctorData = JsonSerializer.Deserialize<DoctorSummaryAIDto>(
            DoctorRecord.SummaryText, JsonOptions) ?? new DoctorSummaryAIDto();

        if (Dto.Subjective is not null) DoctorData.Subjective = Dto.Subjective;
        if (Dto.Objective is not null) DoctorData.Objective = Dto.Objective;
        if (Dto.Assessment is not null) DoctorData.Assessment = Dto.Assessment;
        if (Dto.Plan is not null) DoctorData.Plan = Dto.Plan;
        if (Dto.ComparisonWithPreviousVisit is not null)
            DoctorData.ComparisonWithPreviousVisit = Dto.ComparisonWithPreviousVisit;
        DoctorRecord.SummaryText = JsonSerializer.Serialize(DoctorData);

        // ── Patient-side (Arabic) ─────────────────────────────────────────────
        if (Dto.Diagnosis is not null || Dto.Symptoms is not null ||
     Dto.TreatmentPlan is not null || Dto.WhenToSeekHelp is not null ||
     Dto.FollowUp is not null)   
        {
            var PatientRecord = await _AISummaryRepository.GetByVisitAndTypeAsync(VisitId, "Patient");
            if (PatientRecord is not null)
            {
                var PatientData = JsonSerializer.Deserialize<PatientSummaryAIDto>(
                    PatientRecord.SummaryText, JsonOptions) ?? new PatientSummaryAIDto();

                if (Dto.Diagnosis is not null) PatientData.Diagnosis = Dto.Diagnosis;
                if (Dto.Symptoms is not null) PatientData.Symptoms = Dto.Symptoms;
                if (Dto.TreatmentPlan is not null) PatientData.TreatmentPlan = Dto.TreatmentPlan;
                if (Dto.WhenToSeekHelp is not null) PatientData.WhenToSeekHelp = Dto.WhenToSeekHelp;
                if (Dto.FollowUp is not null)
                    PatientData.FollowUp = Dto.FollowUp;
                PatientRecord.SummaryText = JsonSerializer.Serialize(PatientData);
            }
        }

        await _AISummaryRepository.SaveAsync();
        _Logger.LogInformation("Doctor edited draft summary for VisitId {VisitId}.", VisitId);
    }

    // ── POST /api/visits/{id}/summary/approve ────────────────────────────────
    // Writes everything to permanent columns and tables.
    public async Task ApproveSummaryAsync(int VisitId)
    {
        var DoctorRecord = await _AISummaryRepository.GetByVisitAndTypeAsync(VisitId, "Doctor")
            ?? throw new KeyNotFoundException($"No Doctor summary found for VisitId {VisitId}.");

        var PatientRecord = await _AISummaryRepository.GetByVisitAndTypeAsync(VisitId, "Patient");

        var DoctorData = JsonSerializer.Deserialize<DoctorSummaryAIDto>(
            DoctorRecord.SummaryText, JsonOptions)
            ?? throw new InvalidOperationException("Doctor summary JSON is corrupt.");

        var PatientData = PatientRecord is not null
            ? JsonSerializer.Deserialize<PatientSummaryAIDto>(
                PatientRecord.SummaryText, JsonOptions) ?? new PatientSummaryAIDto()
            : new PatientSummaryAIDto();

        var Visit = await _VisitRepository.GetByIdAsync(VisitId)
            ?? throw new KeyNotFoundException($"Visit {VisitId} not found.");

        // ── 1. Write SOAP → Visit ─────────────────────────────────────────────
        Visit.Subjective = DoctorData.Subjective;
        Visit.Objective = DoctorData.Objective;
        Visit.Assessment = DoctorData.Assessment;
        Visit.Plan = DoctorData.Plan;

        // ── 2. Write patient fields → Visit ──────────────────────────────────
        Visit.Symptoms = PatientData.Symptoms;
        Visit.WhenToSeekHelp = PatientData.WhenToSeekHelp;
        Visit.FollowUp = PatientData.FollowUp; 

        // ── 3. Diagnosis → Diagnosis table ───────────────────────────────────
        // Only add if not already present (guard against double approval)
        if (!string.IsNullOrWhiteSpace(PatientData.Diagnosis))
        {
            var AlreadyExists = await _DiagnosisRepository
                .ExistsForVisitAsync(VisitId, PatientData.Diagnosis);

            if (!AlreadyExists)
            {
                await _DiagnosisRepository.AddAsync(new Diagnosis
                {
                    DiagnosisName = PatientData.Diagnosis,
                    VisitID = VisitId
                });
            }
        }

        // ── 4. TreatmentPlan → Prescription.Instructions ─────────────────────
        // Create prescription if none exists, append to existing if it does.
        if (!string.IsNullOrWhiteSpace(PatientData.TreatmentPlan))
        {
            var Existing = await _PrescriptionRepository.GetByVisitIdAsync(VisitId);
            if (Existing is null)
            {
                await _PrescriptionRepository.AddAsync(new Prescription
                {
                    VisitID = VisitId,
                    Instructions = PatientData.TreatmentPlan
                });
            }
            else if (string.IsNullOrWhiteSpace(Existing.Instructions))
            {
                Existing.Instructions = PatientData.TreatmentPlan;
            }
            else
            {
                // Append AI plan below existing manual instructions
                Existing.Instructions =
                    $"{Existing.Instructions}\n\n{PatientData.TreatmentPlan}";
            }
        }

        await _VisitRepository.SaveChangesAsync();
        _Logger.LogInformation(
    "Summary approved for VisitId {VisitId}. FollowUp={FollowUp}",
    VisitId, PatientData.FollowUp);
    }

    // ── GET /api/visits/{id}/patient-summary ─────────────────────────────────
    // Only accessible after the doctor approves (Visit.Subjective populated).
    public async Task<PatientSummaryViewDto> GetPatientSummaryAsync(int VisitId)
    {
        var Visit = await _VisitRepository.GetByIdAsync(VisitId)
            ?? throw new KeyNotFoundException($"Visit {VisitId} not found.");

        if (string.IsNullOrWhiteSpace(Visit.Subjective))
            throw new InvalidOperationException(
                "Summary has not been approved by the doctor yet.");

        // Diagnosis — read from the Diagnoses table (set on approval)
        var Diagnoses = await _DiagnosisRepository.GetByVisitIdAsync(VisitId);
        var DiagnosisText = string.Join("، ", Diagnoses.Select(D => D.DiagnosisName));

        // TreatmentPlan — read from Prescription.Instructions
        var Prescription = await _PrescriptionRepository.GetByVisitIdAsync(VisitId);

        return new PatientSummaryViewDto
        {
            VisitId = Visit.VisitID,
            VisitDate = Visit.VisitDate,
            Diagnosis = DiagnosisText,
            Symptoms = Visit.Symptoms ?? string.Empty,
            TreatmentPlan = Prescription?.Instructions ?? string.Empty,
            WhenToSeekHelp = Visit.WhenToSeekHelp ?? string.Empty,
            FollowUp = Visit.FollowUp ?? string.Empty
        };
    }

    // ── POST /api/visits/{id}/summary/rating ─────────────────────────────────
    // Optional — doctor rates the AI quality (1–5) for model improvement.
    public async Task<RateSummaryResponseDto> RateSummaryAsync(int VisitId, RateSummaryDto Dto)
    {
        // Rate the Doctor summary row (the one the doctor reviewed and approved)
        var DoctorRecord = await _AISummaryRepository.GetByVisitAndTypeAsync(VisitId, "Doctor")
            ?? throw new KeyNotFoundException($"No summary found for VisitId {VisitId}.");

        DoctorRecord.DoctorRating = Dto.Rating;
        DoctorRecord.DoctorFeedback = Dto.Feedback;

        await _AISummaryRepository.SaveAsync();

        _Logger.LogInformation(
            "Doctor rated summary for VisitId {VisitId}. Rating={Rating}",
            VisitId, Dto.Rating);

        return new RateSummaryResponseDto
        {
            AISummaryID = DoctorRecord.AISummaryID,
            Rating = Dto.Rating,
            Feedback = Dto.Feedback,
            Message = "Rating saved. Thank you for your feedback."
        };
    }
}