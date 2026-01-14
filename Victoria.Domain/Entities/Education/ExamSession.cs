using System;
using Victoria.Domain.Entities.Education;

namespace Victoria.Domain.Entities.Education;

public class ExamSession
{
    public int Id { get; set; }

    public int ExamId { get; set; }
    public Exam Exam { get; set; } = default!;

    public DateTime SessionDate { get; set; }          // termin egzaminu
    public string? Location { get; set; }              // np. London / Online
    public int Capacity { get; set; }                  // limit miejsc

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
