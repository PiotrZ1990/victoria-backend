using System;
using Victoria.Domain.Enums;

namespace Victoria.Domain.Entities.Education
{
    public class Exam
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;     // np. IELTS Academic
        public ExamType ExamType { get; set; }           // enum
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
