namespace Victoria.Domain.Entities.Payments
{
    public class InvoicePayment
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public int PaymentId { get; set; }
        public Payment Payment { get; set; }
    }
}
