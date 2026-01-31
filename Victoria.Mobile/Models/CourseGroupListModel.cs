namespace Victoria.Mobile.Models;

public class CourseGroupListModel
{
    public int Id { get; set; }
    public int LanguageCourseId { get; set; }
    public string GroupName { get; set; } = "";
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int Capacity { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
