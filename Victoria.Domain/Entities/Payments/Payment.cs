using Victoria.Domain.Entities.Cases;
using Victoria.Domain.Enums;

namespace Victoria.Domain.Entities.Payments
{
    public class Payment
    {
        public int Id { get; set; }

        public int CaseFileId { get; set; }
        public CaseFile CaseFile { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; }      // GBP, EUR, USD
        public PaymentMethod PaymentMethod { get; set; } // Card, BankTransfer, Cash

        public ServiceType ServiceType { get; set; }   // Application, Visa, Accommodation

        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; }         // Pending, Paid, Cancelled

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
