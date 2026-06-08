//using CareNota.DTOs.AISummary;
using CareNota.DTOs.Audio;
using Microsoft.AspNetCore.Http;
namespace CareNota.Services.Interfaces;


// ══════════════════════════════════════════════════════════════════════════════
// IAudioService
// ═══════════════════════════using CareNota.DTOs.Audio;


public interface IAudioService
{
    /// <summary>
    /// Validates the file, uploads it to Azure Blob Storage,
    /// persists an AudioRecord, then fires AIService.ProcessAudioAsync()
    /// in the background (non-blocking).
    /// </summary>
    Task<AudioRecordResponseDto> UploadAudioAsync(IFormFile File, int VisitId);
}
