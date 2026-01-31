namespace Victoria.Mobile.Models;

public class EnrollmentListItemModel
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public int CourseGroupId { get; set; }

    public string CourseName { get; set; } = "";
    public string GroupName { get; set; } = "";

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Location { get; set; }

    public DateTime CreatedAt { get; set; }
}
