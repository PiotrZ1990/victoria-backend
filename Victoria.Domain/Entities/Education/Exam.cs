using Victoria.Domain.Enums;

namespace Victoria.Domain.Entities.Education
{
    public class Exam
    {
        public int Id { get; set; }

        public string Name { get; set; }         // PTE, IELTS, Internal Test
        public ExamType ExamType { get; set; }     // External, Internal

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
