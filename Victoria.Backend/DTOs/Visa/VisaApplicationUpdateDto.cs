namespace Victoria.Backend.DTOs.Visa;

public class VisaApplicationUpdateDto
{
    public string Country { get; set; } = default!;
    public string VisaType { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime? AppointmentDate { get; set; }
    public DateTime? DecisionDate { get; set; }
}
