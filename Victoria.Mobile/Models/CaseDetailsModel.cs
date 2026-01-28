namespace Victoria.Mobile.Models;

public class CaseDetailsModel
{
    public int Id { get; set; }
    public string? ClientName { get; set; }
    public string? Status { get; set; }
    public string? Stage { get; set; }
    public DateTime CreatedAt { get; set; }
}
