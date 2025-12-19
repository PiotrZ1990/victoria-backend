namespace Victoria.Backend.DTOs.Reports;

public class PaymentsByServiceDto
{
    public string ServiceType { get; set; }
    public decimal TotalAmount { get; set; }
}