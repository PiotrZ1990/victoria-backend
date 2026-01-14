namespace Victoria.Web.Models.Education;

public class ExamSessionListViewModel
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public string ExamName { get; set; } = default!;
    public DateTime SessionDate { get; set; }
    public string? Location { get; set; }
    public int Capacity { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
