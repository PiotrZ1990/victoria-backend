namespace Victoria.Web.Models.Reports;

public class ReportsPaymentsViewModel
{
    public List<RevenueByMonthViewModel> RevenueMonthly { get; set; } = new();
    public List<PaymentsByServiceViewModel> ByService { get; set; } = new();
}
