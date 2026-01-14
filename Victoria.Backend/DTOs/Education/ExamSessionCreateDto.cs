namespace Victoria.Backend.DTOs.Education;

public class ExamSessionCreateDto
{
    public int ExamId { get; set; }
    public DateTime SessionDate { get; set; }
    public string? Location { get; set; }
    public int Capacity { get; set; } = 20;
    public bool IsActive { get; set; } = true;
}
