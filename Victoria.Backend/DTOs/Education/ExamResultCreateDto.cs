namespace Victoria.Backend.DTOs.Education;

public class ExamResultCreateDto
{
    public int ExamSessionId { get; set; }
    public int StudentId { get; set; }
    public decimal Score { get; set; }
    public string? Notes { get; set; }
}
