namespace Victoria.Backend.DTOs.Cms;

public class PageSectionDetailsDto
{
    public int Id { get; set; }
    public int PageId { get; set; }
    public string SectionKey { get; set; } = default!;
    public string? Title { get; set; }
    public string? Content { get; set; }
    public int Order { get; set; }
    public DateTime UpdatedAt { get; set; }
}
