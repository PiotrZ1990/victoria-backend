namespace Victoria.Mobile.Models;

public class CourseGroupUiModel
{
    public int Id { get; set; }
    public string GroupName { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Capacity { get; set; }
    public bool IsActive { get; set; }

    // UI / enrollment state
    public bool IsEnrolled { get; set; }
    public int? EnrollmentId { get; set; }
}
