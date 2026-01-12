using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Cms;

public class TestimonialEditViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string AuthorName { get; set; } = default!;

    [MaxLength(200)]
    public string? AuthorTitle { get; set; }
    
    [MaxLength(200)]
    public string? Country { get; set; }

    [Required, MaxLength(4000)]
    public string Content { get; set; } = default!;

    [Range(1, 5)]
    public int Rating { get; set; }

    public bool IsPublished { get; set; }
}
