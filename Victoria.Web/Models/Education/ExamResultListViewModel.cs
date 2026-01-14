namespace Victoria.Web.Models.Education;

public class ExamResultListViewModel
{
    public int Id { get; set; }
    public int ExamSessionId { get; set; }
    public int StudentId { get; set; }
    public decimal Score { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}
