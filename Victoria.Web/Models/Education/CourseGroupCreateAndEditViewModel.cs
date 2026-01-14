using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Education;

public class CourseGroupCreateAndEditViewModel
{
    [Required]
    public int LanguageCourseId { get; set; }

    [MaxLength(100)]
    public string? GroupName { get; set; }

    [Required]
    public DateTime StartDate { get; set; } = DateTime.UtcNow.Date;

    [Required]
    public DateTime EndDate { get; set; } = DateTime.UtcNow.Date.AddDays(30);

    [Range(1, 9999)]
    public int Capacity { get; set; } = 10;

    [MaxLength(200)]
    public string? Location { get; set; }

    public bool IsActive { get; set; } = true;
}

public class CourseGroupEditViewModel : CourseGroupCreateAndEditViewModel
{
    public int Id { get; set; }
}
