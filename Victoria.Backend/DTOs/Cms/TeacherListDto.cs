namespace Victoria.Backend.DTOs.Cms;

public class TeacherListDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = default!;
    public string? Title { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
