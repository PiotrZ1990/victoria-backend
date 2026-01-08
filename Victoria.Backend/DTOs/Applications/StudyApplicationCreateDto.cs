namespace Victoria.Backend.DTOs.Applications;

public class StudyApplicationCreateDto
{
    public int CaseFileId { get; set; }
    public string Country { get; set; } = default!;
    public string UniversityName { get; set; } = default!;
    public string ProgramName { get; set; } = default!;
}
