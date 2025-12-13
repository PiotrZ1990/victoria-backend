using Victoria.Domain.Entities.Cases;
using Victoria.Domain.Enums;

namespace Victoria.Domain.Entities.Visa
{
    public class VisaApplication
    {
        public int Id { get; set; }

        // Każda aplikacja wizowa jest powiązana ze sprawą klienta
        public int CaseFileId { get; set; }
        public CaseFile CaseFile { get; set; }

        public string Country { get; set; }          // UK, Canada, Australia
        public VisaType VisaType { get; set; }
        public ApplicationStatus Status { get; set; }           // Draft, Submitted, Approved, Rejected

        public DateTime? AppointmentDate { get; set; }
        public DateTime? DecisionDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
