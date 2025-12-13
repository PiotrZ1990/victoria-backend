using Victoria.Domain.Entities.Cases;
using Victoria.Domain.Enums;

namespace Victoria.Domain.Entities.Applications
{
    public class StudyApplication
    {
        public int Id { get; set; }

        public int CaseFileId { get; set; }
        public CaseFile CaseFile { get; set; }

        // gdzie aplikujemy
        public string Country { get; set; }      // np. UK
        public string UniversityName { get; set; }
        public string ProgramName { get; set; }  // np. Business Management

        public DateTime SubmittedAt { get; set; }
        public ApplicationStatus Status { get; set; }       // Draft, Submitted, Accepted, Rejected

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
