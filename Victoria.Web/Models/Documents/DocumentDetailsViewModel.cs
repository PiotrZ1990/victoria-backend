namespace Victoria.Web.Models.Documents;

public class DocumentDetailsViewModel
{
    public int Id { get; set; }
    public string DocumentType { get; set; } = default!;
    public string? Description { get; set; }
    public int Status { get; set; }  // bo API zwraca 1,2,3...
    public DateTime CreatedAt { get; set; }

    public FileVm File { get; set; } = new();

    public class FileVm
    {
        public int FileResourceId { get; set; }
        public string FileName { get; set; } = default!;
        public string ContentType { get; set; } = default!;
        public long FileSize { get; set; }
        public string FilePath { get; set; } = default!;
    }
}
