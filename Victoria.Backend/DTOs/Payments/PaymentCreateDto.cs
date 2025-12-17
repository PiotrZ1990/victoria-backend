namespace Victoria.Backend.DTOs.Payments;

public class PaymentCreateDto
{
    public int CaseFileId { get; set; }
    public int InvoiceId { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; }

    public string PaymentMethod { get; set; }
    public string ServiceType { get; set; }
}
