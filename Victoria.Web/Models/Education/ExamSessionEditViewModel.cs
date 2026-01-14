using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Education;

public class ExamSessionEditViewModel
{
    public int Id { get; set; }

    [Required]
    public int ExamId { get; set; }

    [Required]
    public DateTime SessionDate { get; set; }

    [MaxLength(200)]
    public string? Location { get; set; }

    [Range(0, 100000)]
    public int Capacity { get; set; }

    public bool IsActive { get; set; }

    public List<ExamLookupViewModel> Exams { get; set; } = new();
}

