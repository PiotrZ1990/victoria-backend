using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Victoria.Backend.DTOs.Files;

public class ImageUploadRequestDto
{
    [Required]
    public IFormFile File { get; set; } = default!;
}
