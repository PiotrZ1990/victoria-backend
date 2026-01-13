namespace Victoria.Web.Models.Cms;

public class TeacherListViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = default!;
    public string? Title { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
