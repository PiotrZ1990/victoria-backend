namespace Victoria.Domain.Entities.Documents
{
    public class VisaDocumentChecklist
    {
        public int Id { get; set; }

        public string DocumentType { get; set; } // Passport, BankStatement
        public bool IsRequired { get; set; }
    }
}
