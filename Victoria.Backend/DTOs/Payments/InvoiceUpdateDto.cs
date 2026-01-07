namespace Victoria.Backend.DTOs.Payments;

public class InvoiceUpdateDto
{
    public string InvoiceNumber { get; set; } = default!;
    public DateTime IssueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = default!;
    public string Status { get; set; } = default!;
}
