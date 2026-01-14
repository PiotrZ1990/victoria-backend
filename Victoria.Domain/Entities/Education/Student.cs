namespace Victoria.Domain.Entities.Education;

public class Student
{
    public int Id { get; set; }

    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;

    public string? Email { get; set; }
    public string? Phone { get; set; }

    public string? Nationality { get; set; }
    public DateTime? DateOfBirth { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
