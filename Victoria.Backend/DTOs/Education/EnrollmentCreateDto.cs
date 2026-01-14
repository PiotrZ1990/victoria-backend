namespace Victoria.Backend.DTOs.Education;

public class EnrollmentCreateDto
{
    public int StudentId { get; set; }
    public int CourseGroupId { get; set; }
    public string Status { get; set; } = "Active";
    public string? Notes { get; set; }
}
