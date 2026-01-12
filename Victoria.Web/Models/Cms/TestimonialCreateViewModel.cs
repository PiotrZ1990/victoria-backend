using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Cms;

public class TestimonialCreateViewModel
{
    [Required, MaxLength(200)]
    public string AuthorName { get; set; } = default!;

    [MaxLength(200)]
    public string? AuthorTitle { get; set; }
    [MaxLength(200)]
    public string? Country { get; set; }

    [Required, MaxLength(4000)]
    public string Content { get; set; } = default!;

    [Range(1, 5)]
    public int Rating { get; set; } = 5;

    public bool IsPublished { get; set; } = false;
}
