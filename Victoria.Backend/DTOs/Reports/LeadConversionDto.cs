namespace Victoria.Backend.DTOs.Reports;

public class LeadConversionDto
{
    public int TotalLeads { get; set; }
    public int ConvertedLeads { get; set; }
    public double ConversionRate { get; set; }
}
