namespace Victoria.Web.Models.Reports;

public class ReportsInvoicesViewModel
{
    public List<InvoiceStatusSummaryViewModel> StatusSummary { get; set; } = new();
    public List<OverdueInvoiceViewModel> Overdue { get; set; } = new();
    public List<OutstandingInvoiceViewModel> Outstanding { get; set; } = new();

    public int Days { get; set; } = 14;
}
