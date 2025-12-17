namespace Victoria.Backend.DTOs.Payments;

public class InvoiceCreateDto
{
    public int CaseFileId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; }
}
