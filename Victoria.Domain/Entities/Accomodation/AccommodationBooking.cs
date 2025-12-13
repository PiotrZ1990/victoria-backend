namespace Victoria.Domain.Entities.Accommodation
{
    public class AccommodationBooking
    {
        public int Id { get; set; }

        public int AccommodationRequestId { get; set; }
        public AccommodationRequest AccommodationRequest { get; set; }

        public string ProviderName { get; set; }
        public string Address { get; set; }

        public DateTime MoveInDate { get; set; }
        public DateTime? MoveOutDate { get; set; }

        public decimal MonthlyPrice { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
