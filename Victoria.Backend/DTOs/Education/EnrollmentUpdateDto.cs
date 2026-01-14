namespace Victoria.Backend.DTOs.Education;

public class EnrollmentUpdateDto
{
    public string Status { get; set; } = "Active";
    public string? Notes { get; set; }
}
