namespace Victoria.Web.Models.Cms;

public class SettingListViewModel
{
    public int Id { get; set; }
    public string Key { get; set; } = default!;
    public string? Value { get; set; }
    public DateTime UpdatedAt { get; set; }
}
