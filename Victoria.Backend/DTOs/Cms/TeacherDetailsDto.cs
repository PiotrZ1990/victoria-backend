namespace Victoria.Backend.DTOs.Cms;

public class TeacherDetailsDto
{
    public int Id { get; set; }
    public string FullName { get; set; } = default!;
    public string? Title { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Bio { get; set; }
    public string? PhotoUrl { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
