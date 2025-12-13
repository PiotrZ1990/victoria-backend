namespace Victoria.Domain.Entities.CRM
{
    public class Lead
    {
        public int Id { get; set; }

        // Kto się zgłosił (formularz / telefon / email)
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        // Skąd lead i czego chce
        public string Source { get; set; }           // np. Website, Facebook, Referral
        public string InterestedCountry { get; set; } // np. UK, Canada
        public string InterestedService { get; set; } // np. University, Visa, Accommodation

        // Status CRM
        public string Status { get; set; }           // New, Contacted, Qualified, Converted, Lost

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
