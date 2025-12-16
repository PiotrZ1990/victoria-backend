using System.ComponentModel.DataAnnotations;

namespace Victoria.Backend.DTOs.Crm;

public class LeadCreateDto
{
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = null!;

    [EmailAddress]
    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [MaxLength(100)]
    public string? Source { get; set; }

    [MaxLength(100)]
    public string? InterestedCountry { get; set; }

    [MaxLength(100)]
    public string? InterestedService { get; set; }
}
