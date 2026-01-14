using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Education;

public class EnrollmentCreateViewModel
{
    [Required]
    public int StudentId { get; set; }

    [Required]
    public int CourseGroupId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Active";

    [MaxLength(500)]
    public string? Notes { get; set; }
}

public class EnrollmentEditViewModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Status { get; set; } = "Active";

    [MaxLength(500)]
    public string? Notes { get; set; }
}
