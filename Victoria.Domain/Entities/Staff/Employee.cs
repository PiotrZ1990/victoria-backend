namespace Victoria.Domain.Entities.Staff
{
    public class Employee
    {
        public int Id { get; set; }

        public string FullName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string PhoneNumber { get; set; } = default!;
        public string Position { get; set; } = default!; // np. Consultant, Admin, Manager

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
