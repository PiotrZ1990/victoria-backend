namespace Victoria.Backend.DTOs.Payments;

public class InvoiceGetDto
{
    public int Id { get; set; }
    public int CaseFileId { get; set; }
    public string InvoiceNumber { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; }
    public string Status { get; set; }
    public DateTime IssueDate { get; set; }
}
