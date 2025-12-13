using Victoria.Domain.Entities.Cases;
using Victoria.Domain.Enums;

namespace Victoria.Domain.Entities.Payments
{
    public class Invoice
    {
        public int Id { get; set; }

        public int CaseFileId { get; set; }
        public CaseFile CaseFile { get; set; }

        public string InvoiceNumber { get; set; }
        public DateTime IssueDate { get; set; }

        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }

        public InvoiceStatus Status { get; set; } // Issued, Paid, Overdue

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
