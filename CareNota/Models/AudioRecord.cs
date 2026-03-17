namespace CareNota.Models;

public class AudioRecord
{
    public int AudioID { get; set; }
    public string AudioFileURL { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime DeletionAt { get; set; }

    // FK
    public int VisitID { get; set; }

    // Navigation
    public Visit Visit { get; set; } = null!;
}