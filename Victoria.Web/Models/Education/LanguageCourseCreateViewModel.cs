using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Education;

public class LanguageCourseCreateViewModel
{
    [Required, MaxLength(200)]
    public string Name { get; set; } = default!;

    [Required, MaxLength(100)]
    public string Language { get; set; } = default!;

    [Required, MaxLength(50)]
    public string Level { get; set; } = default!;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [Range(0, 999999)]
    public decimal Price { get; set; }

    [Required, MaxLength(10)]
    public string Currency { get; set; } = "GBP";

    public bool IsActive { get; set; } = true;
}
