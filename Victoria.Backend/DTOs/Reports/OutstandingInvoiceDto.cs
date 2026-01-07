namespace Victoria.Backend.DTOs.Reports;

public class OutstandingInvoiceDto
{
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = default!;
    public int CaseFileId { get; set; }

    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }

    public string Currency { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime IssueDate { get; set; }
}
