namespace Victoria.Web.Models.Applications;

public class StudyApplicationEditViewModel
{
    public int Id { get; set; }
    public int CaseFileId { get; set; }

    public string Country { get; set; } = default!;
    public string UniversityName { get; set; } = default!;
    public string ProgramName { get; set; } = default!;

    // trzymamy jako string, bo backend przyjmuje string i mapuje na enum
    public string Status { get; set; } = "Draft";
}
