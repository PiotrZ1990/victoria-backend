namespace Victoria.Backend.DTOs.Education;

public class ExamUpdateDto
{
    public string Name { get; set; } = default!;
    public string ExamType { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
}
