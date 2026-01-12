namespace Victoria.Backend.DTOs.Cms;

public class TestimonialListDto
{
    public int Id { get; set; }
    public string AuthorName { get; set; } = default!;
    public string Country { get; set; }
    public int Rating { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
}
