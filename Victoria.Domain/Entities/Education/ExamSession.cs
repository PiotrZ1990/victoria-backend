namespace Victoria.Domain.Entities.Education
{
    public class ExamSession
    {
        public int Id { get; set; }

        public int ExamId { get; set; }
        public Exam Exam { get; set; }

        public DateTime ExamDate { get; set; }
        public string Location { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
