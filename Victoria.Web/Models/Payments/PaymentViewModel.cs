namespace Victoria.Web.Models.Payments;

public class PaymentViewModel
{
    public int Id { get; set; }
    public int CaseFileId { get; set; }
    public int InvoiceId { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; }

    public string PaymentMethod { get; set; }
    public string ServiceType { get; set; }

    public string Status { get; set; }
    public DateTime PaymentDate { get; set; }
}
