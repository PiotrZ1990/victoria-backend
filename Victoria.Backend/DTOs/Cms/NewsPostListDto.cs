namespace Victoria.Backend.DTOs.Cms;

public class NewsPostListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
