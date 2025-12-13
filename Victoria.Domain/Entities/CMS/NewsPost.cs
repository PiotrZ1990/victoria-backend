namespace Victoria.Domain.Entities.CMS
{
    public class NewsPost
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public string Content { get; set; }

        public DateTime PublishDate { get; set; }
        public bool IsPublished { get; set; }
    }
}
