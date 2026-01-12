namespace Victoria.Backend.DTOs.Cms;

public class NewsPostCreateDto
{
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public bool IsPublished { get; set; }
}
