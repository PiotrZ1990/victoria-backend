namespace Victoria.Backend.DTOs.Cms;

public class SettingUpdateDto
{
    public string Key { get; set; } = default!;
    public string? Value { get; set; }
    public string? Description { get; set; }
}
