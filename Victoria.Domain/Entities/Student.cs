namespace Victoria.Domain.Entities
{
    public class Student
    {
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
