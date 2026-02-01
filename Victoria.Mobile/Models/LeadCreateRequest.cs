namespace Victoria.Mobile.Models;

public class LeadCreateRequest
{
    public string FullName { get; set; } = default!;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public string? Source { get; set; }
    public string? InterestedCountry { get; set; }
    public string? InterestedService { get; set; }
}
