using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Education;

public class ExamSessionCreateViewModel
{
    [Required]
    public int ExamId { get; set; }

    [Required]
    public DateTime SessionDate { get; set; } = DateTime.UtcNow.Date.AddDays(7);

    [MaxLength(200)]
    public string? Location { get; set; }

    [Range(0, 100000)]
    public int Capacity { get; set; } = 20;

    public bool IsActive { get; set; } = true;

    // do dropdowna
    public List<ExamLookupViewModel> Exams { get; set; } = new();

}
