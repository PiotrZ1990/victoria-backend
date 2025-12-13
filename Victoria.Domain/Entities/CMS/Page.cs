namespace Victoria.Domain.Entities.CMS
{
    public class Page
    {
        public int Id { get; set; }

        public string Slug { get; set; }        // np. about-us, services
        public string Title { get; set; }

        public bool IsPublished { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
