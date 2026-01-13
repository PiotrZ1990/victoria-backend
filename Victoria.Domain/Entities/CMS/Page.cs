namespace Victoria.Domain.Entities.Cms;

public class Page
{
    public int Id { get; set; }

    public string Title { get; set; } = default!;
    public string Slug { get; set; } = default!;   // np. "about", "contact"
    public bool IsPublished { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public ICollection<PageSection> Sections { get; set; } = new List<PageSection>();
}
