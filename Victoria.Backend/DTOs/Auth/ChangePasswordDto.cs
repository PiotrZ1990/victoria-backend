using System.ComponentModel.DataAnnotations;

namespace Victoria.Backend.DTOs.Auth;

public class ChangePasswordDto
{
    [Required]
    public string CurrentPassword { get; set; } = default!;

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; } = default!;
}
