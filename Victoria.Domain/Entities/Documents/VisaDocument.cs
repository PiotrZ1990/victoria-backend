using Victoria.Domain.Entities.Visa;

namespace Victoria.Domain.Entities.Documents
{
    public class VisaDocument
    {
        public int Id { get; set; }

        public int VisaApplicationId { get; set; }
        public VisaApplication VisaApplication { get; set; }

        public int DocumentId { get; set; }
        public Document Document { get; set; }
    }
}
