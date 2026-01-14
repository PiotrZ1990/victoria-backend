namespace Victoria.Backend.DTOs.Education;

public class CourseGroupUpdateDto
{
    public int LanguageCourseId { get; set; }
    public string? GroupName { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Capacity { get; set; }
    public string? Location { get; set; }
    public bool IsActive { get; set; } = true;
}
