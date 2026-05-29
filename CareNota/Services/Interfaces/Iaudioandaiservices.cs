//using CareNota.DTOs.AISummary;
using CareNota.DTOs.Audio;

namespace CareNota.Services.Interfaces;

// ══════════════════════════════════════════════════════════════════════════════
// IAudioService
// ══════════════════════════════════════════════════════════════════════════════
public interface IAudioService
{
    // Upload audio to Azure Blob, save AudioRecord, trigger AI processing
    Task<AudioRecordDto> UploadAsync(int VisitId, IFormFile AudioFile);

    // Get the AudioRecord for a visit (status check)
    Task<AudioRecordDto?> GetByVisitIdAsync(int VisitId);

    // Called by the background job — deletes blob + DB row
    Task DeleteExpiredAudioAsync();
}

//// ══════════════════════════════════════════════════════════════════════════════
//// IAIService
//// ══════════════════════════════════════════════════════════════════════════════
//public interface IAIService
//{
//    // Sends audioUrl + visitId to Python FastAPI, saves both summaries to DB
//    Task ProcessAudioAsync(string AudioUrl, int VisitId);

//    // Get all summaries for a visit
//    Task<IEnumerable<AISummaryDto>> GetSummariesAsync(int VisitId);

//    // Doctor edits the summary text before approving
//    Task<AISummaryDto> UpdateSummaryAsync(int SummaryId, UpdateAISummaryDto Dto);

//    // Doctor rates the summary quality (1–5)
//    Task<AISummaryDto> RateSummaryAsync(int SummaryId, RateAISummaryDto Dto);
//}