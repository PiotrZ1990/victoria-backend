using Victoria.Domain.Entities.Files;

namespace Victoria.Domain.Entities.Documents
{
    public class Document
    {
        public int Id { get; set; }

        public string DocumentType { get; set; }  // SOP, CV, Passport, OfferLetter

        public string? Description { get; set; } // ⬅️ NOWE (opcjonalne)

        public int FileResourceId { get; set; }
        public FileResource FileResource { get; set; }

        public string Status { get; set; }        // Uploaded, Approved, Rejected

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
