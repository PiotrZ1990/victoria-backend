namespace Victoria.Domain.Entities.Documents
{
    public class ApplicationDocumentChecklist
    {
        public int Id { get; set; }

        public string DocumentType { get; set; } // SOP, CV, Transcript
        public bool IsRequired { get; set; }
    }
}
