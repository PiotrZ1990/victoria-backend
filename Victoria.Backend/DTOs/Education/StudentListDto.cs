namespace Victoria.Backend.DTOs.Education;

public class StudentListDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = default!;
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
