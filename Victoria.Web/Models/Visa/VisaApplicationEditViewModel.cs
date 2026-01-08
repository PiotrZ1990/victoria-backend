using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Visa;

public class VisaApplicationEditViewModel
{
    public int Id { get; set; }

    [Required]
    public string Country { get; set; } = default!;

    [Required]
    public string VisaType { get; set; } = default!;

    [Required]
    public string Status { get; set; } = default!;

    public DateTime? AppointmentDate { get; set; }
    public DateTime? DecisionDate { get; set; }
}
