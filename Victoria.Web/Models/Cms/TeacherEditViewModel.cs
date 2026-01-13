using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Cms;

public class TeacherEditViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string FullName { get; set; } = default!;

    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(50)]
    public string? Phone { get; set; }

    [MaxLength(4000)]
    public string? Bio { get; set; }
    public IFormFile? PhotoFile { get; set; }
    public string? PhotoUrl { get; set; } // zostaje, ale będzie ustawiane po uploadzie

    public bool IsActive { get; set; }
}
