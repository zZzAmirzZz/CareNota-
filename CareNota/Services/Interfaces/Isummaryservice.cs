using CareNota.DTOs.Summary;

namespace CareNota.Services.Interfaces;

public interface ISummaryService
{
    /// <summary>
    /// GET /api/visits/{id}/summary
    /// Returns the AI draft for doctor review.
    /// Throws KeyNotFoundException if AI has not finished yet.
    /// </summary>
    Task<VisitSummaryResponseDto> GetSummaryAsync(int VisitId);

    /// <summary>
    /// PUT /api/visits/{id}/summary
    /// Doctor edits any field before approving. Null fields are preserved.
    /// </summary>
    Task EditSummaryAsync(int VisitId, EditSummaryDto Dto);

    /// <summary>
    /// POST /api/visits/{id}/summary/approve
    /// Writes final SOAP + WhenToSeekHelp + FollowUpDate into Visit.
    /// </summary>
    Task ApproveSummaryAsync(int VisitId, ApproveSummaryDto Dto);

    /// <summary>
    /// GET /api/visits/{id}/patient-summary
    /// Returns the patient-facing summary. Only available after doctor approves.
    /// </summary>
    Task<PatientSummaryViewDto> GetPatientSummaryAsync(int VisitId);
}