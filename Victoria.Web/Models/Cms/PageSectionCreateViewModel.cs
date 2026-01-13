using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Cms;

public class PageSectionCreateViewModel
{
    [Required]
    public int PageId { get; set; }

    [Required, MaxLength(100)]
    public string SectionKey { get; set; } = default!;

    [MaxLength(200)]
    public string? Title { get; set; }

    public string? Content { get; set; }

    public int Order { get; set; } = 1;
}
