namespace Victoria.Web.Models.Cms;

public class SettingDetailsViewModel
{
    public int Id { get; set; }
    public string Key { get; set; } = default!;
    public string? Value { get; set; }
    public string? Description { get; set; }
    public DateTime UpdatedAt { get; set; }
}
