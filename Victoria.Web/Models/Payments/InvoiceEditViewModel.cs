using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Payments;

public class InvoiceEditViewModel
{
    public int Id { get; set; }

    [Required]
    public string InvoiceNumber { get; set; } = "";

    [Required]
    public DateTime IssueDate { get; set; }

    [Required]
    [Range(0.01, 999999999)]
    public decimal TotalAmount { get; set; }

    [Required]
    public string Currency { get; set; } = "GBP";

    [Required]
    public string Status { get; set; } = "Issued";
}
