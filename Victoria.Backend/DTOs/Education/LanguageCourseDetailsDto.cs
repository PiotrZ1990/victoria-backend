namespace Victoria.Backend.DTOs.Education;

public class LanguageCourseDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Language { get; set; } = default!;
    public string Level { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = default!;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
