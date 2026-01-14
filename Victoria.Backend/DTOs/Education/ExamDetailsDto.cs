namespace Victoria.Backend.DTOs.Education;

public class ExamDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string ExamType { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
