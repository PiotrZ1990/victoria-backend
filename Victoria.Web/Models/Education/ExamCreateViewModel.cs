using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Education;

public class ExamCreateViewModel
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = default!;

    [Required, MaxLength(50)]
    public string ExamType { get; set; } = "IELTS";

    [MaxLength(2000)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}
