using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Payments;

public class PaymentCreateViewModel
{
    [Required]
    public int InvoiceId { get; set; }

    [Required]
    public int CaseFileId { get; set; }

    [Required]
    [Range(0.01, 999999999)]
    public decimal Amount { get; set; }

    [Required]
    public string Currency { get; set; } = "GBP";

    [Required]
    public string PaymentMethod { get; set; } = "BankTransfer";

    [Required]
    public string ServiceType { get; set; } = "Application";
}
