namespace Victoria.Backend.DTOs.Crm;

public class LeadUpdateDto
{
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Source { get; set; }
    public string InterestedCountry { get; set; }
    public string InterestedService { get; set; }
    public string Status { get; set; }
}
