using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Crm;

public class LeadCreateViewModel
{
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; }

    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; }

    [MaxLength(50)]
    public string PhoneNumber { get; set; }

    [MaxLength(100)]
    public string Source { get; set; }

    [MaxLength(100)]
    public string InterestedCountry { get; set; }

    [MaxLength(100)]
    public string InterestedService { get; set; }
}
