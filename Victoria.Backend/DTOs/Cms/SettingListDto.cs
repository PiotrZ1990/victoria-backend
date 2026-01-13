namespace Victoria.Backend.DTOs.Cms;

public class SettingListDto
{
    public int Id { get; set; }
    public string Key { get; set; } = default!;
    public string? Value { get; set; }
    public DateTime UpdatedAt { get; set; }
}
