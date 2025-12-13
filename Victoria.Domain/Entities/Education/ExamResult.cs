using Victoria.Domain.Entities;

namespace Victoria.Domain.Entities.Education
{
    public class ExamResult
    {
        public int Id { get; set; }

        public int ExamSessionId { get; set; }
        public ExamSession ExamSession { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public decimal Score { get; set; }
        public string Result { get; set; }       // Pass / Fail / Band score

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
