namespace Victoria.Web.Models.Payments;

public class InvoiceDetailsViewModel
{
    public int Id { get; set; }
    public int CaseFileId { get; set; }

    public string InvoiceNumber { get; set; }
    public DateTime IssueDate { get; set; }

    public decimal TotalAmount { get; set; }
    public string Currency { get; set; }

    public string Status { get; set; }

    // Wyliczane w Web
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }

    public List<PaymentViewModel> Payments { get; set; } = new();
}
