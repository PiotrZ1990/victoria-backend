namespace Victoria.Backend.DTOs.Reports;

public class RevenueByMonthDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public decimal TotalRevenue { get; set; }
}
