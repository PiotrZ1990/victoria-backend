using Microsoft.AspNetCore.Http;

namespace Victoria.Backend.DTOs.Documents;

public class DocumentUploadRequest
{
    public IFormFile File { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
}
