namespace Victoria.Mobile.Models;

public class NewsPostDetailsModel
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Summary { get; set; }
    public string? Content { get; set; }
    public string? PhotoUrl { get; set; }
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
