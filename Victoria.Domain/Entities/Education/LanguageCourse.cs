namespace Victoria.Domain.Entities.Education
{
    public class LanguageCourse
    {
        public int Id { get; set; }

        public string Name { get; set; } = default!;          // np. "English B2"
        public string Language { get; set; } = default!;      // np. "English"
        public string Level { get; set; } = default!;         // np. "B2"

        public string? Description { get; set; }

        public decimal Price { get; set; }                   // ważne: precyzja w OnModelCreating
        public string Currency { get; set; } = "GBP";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
