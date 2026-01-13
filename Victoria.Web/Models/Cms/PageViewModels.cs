using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Cms;

public class PageListViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PageDetailsViewModel : PageListViewModel
{
    public DateTime? UpdatedAt { get; set; }
}

public class PageCreateViewModel
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = default!;

    [Required, MaxLength(200)]
    public string Slug { get; set; } = default!;

    public bool IsPublished { get; set; } = true;
}

public class PageEditViewModel : PageCreateViewModel
{
    public int Id { get; set; }
}
