using Victoria.Domain.Entities.Cms;

namespace Victoria.Domain.Entities.CMS
{
    public class PageSection
    {
        public int Id { get; set; }

        public int PageId { get; set; }
        public Page Page { get; set; }

        public string SectionType { get; set; }   // Hero, Text, Image, CTA
        public string Content { get; set; }       // HTML / tekst

        public int Order { get; set; }
    }
}
