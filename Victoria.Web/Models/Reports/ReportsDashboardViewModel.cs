namespace Victoria.Web.Models.Reports;

public class ReportsDashboardViewModel
{
    public LeadConversionViewModel? LeadConversion { get; set; }

    public decimal TotalOutstanding { get; set; }
    public int OutstandingCount { get; set; }

    public List<OutstandingInvoiceViewModel> TopOutstanding { get; set; } = new();
    public List<RevenueByMonthViewModel> RevenueMonthly { get; set; } = new();
}
