using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Victoria.Web.Models.Documents;

public class DocumentUploadVisaViewModel
{
    [Required]
    public int VisaApplicationId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = default!;

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public IFormFile File { get; set; } = default!;
}
