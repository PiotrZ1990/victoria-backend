using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Education;

public class ExamEditViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = default!;

    [Required, MaxLength(50)]
    public string ExamType { get; set; } = default!;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public bool IsActive { get; set; }
}
