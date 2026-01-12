namespace Victoria.Backend.DTOs.Cms;

public class NewsPostUpdateDto
{
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public bool IsPublished { get; set; }
}
