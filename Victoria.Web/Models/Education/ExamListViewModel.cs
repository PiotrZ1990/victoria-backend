namespace Victoria.Web.Models.Education;

public class ExamListViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string ExamType { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
