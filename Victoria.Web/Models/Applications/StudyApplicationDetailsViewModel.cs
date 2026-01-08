namespace Victoria.Web.Models.Applications;

public class StudyApplicationDetailsViewModel
{
    public int Id { get; set; }
    public int CaseFileId { get; set; }

    public string Country { get; set; } = default!;
    public string UniversityName { get; set; } = default!;
    public string ProgramName { get; set; } = default!;

    public string Status { get; set; } = default!;
    public DateTime? SubmittedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
