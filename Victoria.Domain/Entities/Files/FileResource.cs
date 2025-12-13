namespace Victoria.Domain.Entities.Files
{
    public class FileResource
    {
        public int Id { get; set; }

        public string FileName { get; set; }      // nazwa oryginalna
        public string FilePath { get; set; }      // ścieżka lokalna
        public string ContentType { get; set; }   // pdf, docx, jpg

        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
