using System.Text.Json.Serialization;

namespace CareNota.DTOs.Audio;

public class AudioRecordResponseDto
{
    public int AudioId { get; set; }
    public string AudioFileUrl { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime DeletionAt { get; set; }
    public int VisitId { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class AudioUploadDto
{
    public IFormFile AudioFile { get; set; } = null!;
    public int VisitId { get; set; }
}