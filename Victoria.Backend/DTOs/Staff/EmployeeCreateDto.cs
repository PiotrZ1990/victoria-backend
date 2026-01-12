namespace Victoria.Backend.DTOs.Staff;

public class EmployeeCreateDto
{
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string PhoneNumber { get; set; } = default!;
    public string Position { get; set; } = default!;
    public bool IsActive { get; set; } = true;
}
