namespace Victoria.Web.Models.Reports;

public class PaymentsByServiceViewModel
{
    public string ServiceType { get; set; } = default!;
    public decimal TotalAmount { get; set; }
}
