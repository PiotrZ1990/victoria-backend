using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Cms;

public class SettingEditViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Key { get; set; } = default!;

    public string? Value { get; set; }
    public string? Description { get; set; }
}
