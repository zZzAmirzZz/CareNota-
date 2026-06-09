namespace CareNota.Interfaces;

public interface IAIService
{
    /// <summary>
    /// POSTs { "audio_url": "..." } to Python FastAPI /process-audio,
    /// deserialises the nested doctor_summary + patient_summary response,
    /// and saves two draft AISummary rows to the database.
    /// The doctor then reviews via SummaryService (GET → PUT → approve).
    /// </summary>
    Task ProcessAudioAsync(string AudioUrl, int VisitId);

}
