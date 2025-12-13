using Victoria.Domain.Entities.CRM;
using Victoria.Domain.Enums;

namespace Victoria.Domain.Entities.Cases
{
    public class CaseFile
    {
        public int Id { get; set; }

        // Case jest tworzony zwykle z leada
        public int LeadId { get; set; }
        public Lead Lead { get; set; }

        // Identyfikator sprawy do łatwego szukania
        public string CaseNumber { get; set; }

        // Etap sprawy: np. Planning, Applying, Visa, Accommodation, Completed
        public CaseStage Stage { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
