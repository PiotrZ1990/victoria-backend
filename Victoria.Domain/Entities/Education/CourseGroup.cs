using System.ComponentModel.DataAnnotations;

namespace Victoria.Domain.Entities.Education;

public class CourseGroup
{
    public int Id { get; set; }

    public int LanguageCourseId { get; set; }
    public LanguageCourse? LanguageCourse { get; set; }

    [MaxLength(100)]
    public string? GroupName { get; set; }  // np. "Group A", "Evening"

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int Capacity { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
