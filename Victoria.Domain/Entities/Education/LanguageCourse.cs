namespace Victoria.Domain.Entities.Education
{
    public class LanguageCourse
    {
        public int Id { get; set; }

        public string Name { get; set; }            // np. English Preparation
        public string Level { get; set; }           // A1, B2, C1
        public int DurationWeeks { get; set; }

        public decimal Price { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
