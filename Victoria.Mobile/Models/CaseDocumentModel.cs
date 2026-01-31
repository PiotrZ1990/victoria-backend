namespace Victoria.Mobile.Models;

public class CaseDocumentModel
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public CaseDocumentFileModel File { get; set; } = new();
}

public class CaseDocumentFileModel
{
    public int FileResourceId { get; set; }
    public string? FileName { get; set; }
    public string? ContentType { get; set; }
    public long FileSize { get; set; }
    public string? FilePath { get; set; }
}
