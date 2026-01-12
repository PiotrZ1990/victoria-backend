using System.ComponentModel.DataAnnotations;

public class NewsPostCreateViewModel
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    public bool IsPublished { get; set; }
}
