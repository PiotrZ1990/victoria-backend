namespace Victoria.Backend.DTOs.Cms;

public class TestimonialDetailsDto
{
    public int Id { get; set; }
    public string AuthorName { get; set; } = default!;
    public string? AuthorTitle { get; set; }
    public string? Country { get; set; }
    public string Content { get; set; } = default!;
    public int Rating { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
}
