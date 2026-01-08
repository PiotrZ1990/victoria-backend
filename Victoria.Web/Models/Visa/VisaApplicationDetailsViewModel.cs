namespace Victoria.Web.Models.Visa;

public class VisaApplicationDetailsViewModel
{
    public int Id { get; set; }
    public int CaseFileId { get; set; }
    public string Country { get; set; } = default!;
    public string VisaType { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime? AppointmentDate { get; set; }
    public DateTime? DecisionDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
