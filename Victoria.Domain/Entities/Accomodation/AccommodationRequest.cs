using Victoria.Domain.Entities.Cases;

namespace Victoria.Domain.Entities.Accommodation
{
    public class AccommodationRequest
    {
        public int Id { get; set; }

        public int CaseFileId { get; set; }
        public CaseFile CaseFile { get; set; }

        public string Country { get; set; }
        public string City { get; set; }

        public decimal? BudgetMin { get; set; }
        public decimal? BudgetMax { get; set; }

        public string Status { get; set; }   // New, Searching, Offered, Booked, Cancelled

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
