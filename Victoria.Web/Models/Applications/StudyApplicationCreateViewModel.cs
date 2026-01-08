using System.ComponentModel.DataAnnotations;

namespace Victoria.Web.Models.Applications;

public class StudyApplicationCreateViewModel
{
    [Required]
    public int CaseFileId { get; set; }

    [Required]
    public string Country { get; set; } = default!;

    [Required]
    public string UniversityName { get; set; } = default!;

    [Required]
    public string ProgramName { get; set; } = default!;
}
