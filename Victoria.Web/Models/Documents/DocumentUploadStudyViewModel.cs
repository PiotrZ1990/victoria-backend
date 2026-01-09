using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Documents;

public class DocumentUploadStudyViewModel
{
    [Required]
    public int StudyApplicationId { get; set; }

    [Required]
    public string Title { get; set; } = default!;   // DocumentType

    public string? Description { get; set; }

    [Required]
    public IFormFile File { get; set; } = default!;
}
