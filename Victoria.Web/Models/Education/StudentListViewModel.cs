namespace Victoria.Web.Models.Education;

public class StudentListViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = default!;
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
