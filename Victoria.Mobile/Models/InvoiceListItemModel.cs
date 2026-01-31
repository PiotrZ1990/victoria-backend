namespace Victoria.Mobile.Models;

public class InvoiceListItemModel
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = "";
    public DateTime IssueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "";
    public string Status { get; set; } = "";
}
