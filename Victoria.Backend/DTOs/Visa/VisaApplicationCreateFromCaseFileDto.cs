namespace Victoria.Backend.DTOs.Visa;

public class VisaApplicationCreateFromCaseFileDto
{
    public int CaseFileId { get; set; }
    public string Country { get; set; } = "UK";
    public string VisaType { get; set; } = "Student";
}
