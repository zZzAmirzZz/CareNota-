namespace CareNota.DTOs.Audio;

// ── Read ──────────────────────────────────────────────────────────────────────
public class AudioRecordDto
{
    public int AudioID { get; set; }
    public string AudioFileURL { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime DeletionAt { get; set; }
    public int VisitID { get; set; }
    public string Status { get; set; } = string.Empty; // "Processing" | "Done" | "Failed"
}

// ── Write ─────────────────────────────────────────────────────────────────────
public class AudioUploadDto
{
    public IFormFile AudioFile { get; set; } = null!;
}