namespace Victoria.Domain.Entities.CMS
{
    public class Testimonial
    {
        public int Id { get; set; }

        public string AuthorName { get; set; }
        public string? AuthorTitle { get; set; }          // np. "Student"
        public string Country { get; set; }
        public string Content { get; set; }
        public int Rating { get; set; }  // 1–5
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
