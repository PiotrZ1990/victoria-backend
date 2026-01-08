using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Visa;

public class VisaApplicationCreateViewModel
{
    [Required]
    public int CaseFileId { get; set; }

    [Required]
    public string Country { get; set; } = "UK";

    [Required]
    public string VisaType { get; set; } = "Student";

    public string Status { get; set; } = "Draft";

    public DateTime? AppointmentDate { get; set; }
    public DateTime? DecisionDate { get; set; }
}
