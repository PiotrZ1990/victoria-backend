namespace Victoria.Domain.Entities.Cms;

public class PageSection
{
    public int Id { get; set; }

    public int PageId { get; set; }
    public Page Page { get; set; } = default!;

    public string SectionKey { get; set; } = default!; // np. "hero", "about", "cta"
    public string? Title { get; set; }
    public string? Content { get; set; }

    public int Order { get; set; } = 1;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
