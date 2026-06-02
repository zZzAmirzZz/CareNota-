using CareNota.DTOs.Summary;

namespace CareNota.Services.Interfaces;


public interface ISummaryService
{
    Task<VisitSummaryResponseDto> GetSummaryAsync(int VisitId);
    Task EditSummaryAsync(int VisitId, EditSummaryDto Dto);
    Task ApproveSummaryAsync(int VisitId);
    Task<PatientSummaryViewDto> GetPatientSummaryAsync(int VisitId);
    Task<RateSummaryResponseDto> RateSummaryAsync(int VisitId, RateSummaryDto Dto);
}