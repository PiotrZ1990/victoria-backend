using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Education;

public class ExamResultEditViewModel
{
    public int Id { get; set; }

    [Required]
    public int ExamSessionId { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    public decimal Score { get; set; }

    public string? Notes { get; set; }

    // dropdowny (tylko to używamy)
    public List<ExamSessionLookupViewModel> Sessions { get; set; } = new();
    public List<StudentLookupViewModel> Students { get; set; } = new();
}
