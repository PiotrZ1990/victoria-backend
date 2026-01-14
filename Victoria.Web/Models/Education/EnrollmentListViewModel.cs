namespace Victoria.Web.Models.Education;

public class EnrollmentListViewModel
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = default!;
    public int CourseGroupId { get; set; }
    public string CourseName { get; set; } = default!;
    public string? GroupName { get; set; }
    public string Status { get; set; } = default!;
    public DateTime EnrolledAt { get; set; }
}
