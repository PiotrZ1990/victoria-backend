namespace Victoria.Web.Models.Cases;

public class CaseFileViewModel
{
    public int Id { get; set; }

    public string CaseNumber { get; set; }

    // Lead
    public string LeadFullName { get; set; }
    public string LeadEmail { get; set; }

    public string Stage { get; set; }

    public DateTime CreatedAt { get; set; }
}
