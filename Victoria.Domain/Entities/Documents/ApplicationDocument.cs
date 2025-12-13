using Victoria.Domain.Entities.Applications;

namespace Victoria.Domain.Entities.Documents
{
    public class ApplicationDocument
    {
        public int Id { get; set; }

        public int StudyApplicationId { get; set; }
        public StudyApplication StudyApplication { get; set; }

        public int DocumentId { get; set; }
        public Document Document { get; set; }
    }
}
