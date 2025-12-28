namespace Victoria.Backend.DTOs.Documents;

public class DocumentUploadResultDto
{
    public int DocumentId { get; set; }
    public int FileResourceId { get; set; }

    public string Title { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public long SizeBytes { get; set; }

    public string FilePath { get; set; }
    public DateTime CreatedAt { get; set; }
}
