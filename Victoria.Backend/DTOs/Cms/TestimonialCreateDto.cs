namespace Victoria.Backend.DTOs.Cms;

public class TestimonialCreateDto
{
    public string AuthorName { get; set; } = default!;
    public string? AuthorTitle { get; set; }
    public string Country { get; set; }
    public string Content { get; set; } = default!;
    public int Rating { get; set; } = 5;
    public bool IsPublished { get; set; } = false;
}
