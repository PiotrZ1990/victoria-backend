namespace Victoria.Backend.DTOs.Reports;

public class CaseMissingDocsDto
{
    public int CaseFileId { get; set; }
    public string CaseNumber { get; set; } = default!;
    public string Stage { get; set; } = default!;

    public int MissingRequiredApplicationItems { get; set; }
    public int MissingRequiredVisaItems { get; set; }
}
