namespace Victoria.Domain.Entities.Cms;

public class Teacher
{
    public int Id { get; set; }

    public string FullName { get; set; } = default!;
    public string? Title { get; set; }          // np. "IELTS Instructor"
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public string? Bio { get; set; }            // opis
    public string? PhotoUrl { get; set; }       // np. link do zdjęcia

    public bool IsActive { get; set; } = true;  // czy pokazywać w CMS

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
