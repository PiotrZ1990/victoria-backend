using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Victoria.Web.Models.Documents;

public class DocumentUploadViewModel
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = default!; // u Ciebie to DocumentType

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public IFormFile File { get; set; } = default!;
}
