namespace CareNota.Models
{
    public class AudioRecord
    {
        public int AudioID { get; set; }
        public string AudioFileURL { get; set; } // Path to file
        public DateTime CreatedAt { get; set; }

        public DateTime DeletionAt { get; set; }
        public int VisitID { get; set; } // Foreign key to Visit
    }
}
