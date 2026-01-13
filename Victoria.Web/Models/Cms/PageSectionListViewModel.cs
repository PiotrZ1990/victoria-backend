namespace Victoria.Web.Models.Cms;

public class PageSectionListViewModel
{
    public int Id { get; set; }
    public int PageId { get; set; }
    public string SectionKey { get; set; } = default!;
    public string? Title { get; set; }
    public int Order { get; set; }
    public DateTime UpdatedAt { get; set; }
}
