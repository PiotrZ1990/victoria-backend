namespace Victoria.Backend.DTOs.Education;

public class LanguageCourseUpdateDto
{
    public string Name { get; set; } = default!;
    public string Language { get; set; } = default!;
    public string Level { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "GBP";
    public bool IsActive { get; set; } = true;
}
