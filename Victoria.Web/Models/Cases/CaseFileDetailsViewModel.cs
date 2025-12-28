using Victoria.Web.Models.Payments;

namespace Victoria.Web.Models.Cases;

public class CaseFileDetailsViewModel
{
    public int Id { get; set; }
    public string CaseNumber { get; set; }
    public string Stage { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? InternalNotes { get; set; }

    // Lead
    public int LeadId { get; set; }
    public string LeadFullName { get; set; }
    public string LeadEmail { get; set; }
    public string LeadPhoneNumber { get; set; }
    public string LeadSource { get; set; }
    public string LeadInterestedCountry { get; set; }
    public string LeadInterestedService { get; set; }
    public string LeadStatus { get; set; }

    //Invoices
    public List<InvoiceViewModel> Invoices { get; set; } = new();

}

