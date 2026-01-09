namespace Victoria.Web.Models.Documents;

public class DocumentListViewModel
{
    public int Id { get; set; }
    public string DocumentType { get; set; } = default!;
    public string? Description { get; set; }
    public string Status { get; set; } = default!;
    public DateTime CreatedAt { get; set; }

    public DocumentFileViewModel File { get; set; } = new();
}

public class DocumentFileViewModel
{
    public int FileResourceId { get; set; }
    public string FileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long FileSize { get; set; }
    public string FilePath { get; set; } = default!;
}
