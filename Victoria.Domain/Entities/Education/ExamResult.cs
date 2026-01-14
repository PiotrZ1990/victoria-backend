using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Victoria.Domain.Entities.Education;

public class ExamResult
{
    [Key]
    public int Id { get; set; }

    // =========================
    // RELATIONS
    // =========================
    public int ExamSessionId { get; set; }
    public ExamSession ExamSession { get; set; } = default!;

    public int StudentId { get; set; }
    public Student Student { get; set; } = default!;

    // =========================
    // DATA
    // =========================
    [Column(TypeName = "decimal(18,2)")]
    public decimal Score { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
}
