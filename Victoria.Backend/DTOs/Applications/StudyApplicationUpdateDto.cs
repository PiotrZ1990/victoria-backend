namespace Victoria.Backend.DTOs.Applications;

public class StudyApplicationUpdateDto
{
    public string Country { get; set; } = default!;
    public string UniversityName { get; set; } = default!;
    public string ProgramName { get; set; } = default!;
    public string Status { get; set; } = default!; // enum jako string (Draft/Submitted/Approved/Rejected)
}
