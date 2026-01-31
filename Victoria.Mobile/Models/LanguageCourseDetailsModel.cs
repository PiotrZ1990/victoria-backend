namespace Victoria.Mobile.Models;

public class LanguageCourseDetailsModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Level { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "GBP";
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
