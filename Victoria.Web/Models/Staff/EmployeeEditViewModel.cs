using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Staff;

public class EmployeeEditViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string FullName { get; set; } = default!;

    [Required, EmailAddress, MaxLength(200)]
    public string Email { get; set; } = default!;

    [MaxLength(50)]
    public string PhoneNumber { get; set; } = "";

    [Required, MaxLength(100)]
    public string Position { get; set; } = "Consultant";

    public bool IsActive { get; set; } = true;
}
