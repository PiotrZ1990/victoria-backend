namespace Victoria.Backend.DTOs.Cms;

public class PageListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PageDetailsDto : PageListDto
{
    public DateTime? UpdatedAt { get; set; }
}

public class PageCreateDto
{
    public string Title { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public bool IsPublished { get; set; }
}

public class PageUpdateDto : PageCreateDto
{
}
