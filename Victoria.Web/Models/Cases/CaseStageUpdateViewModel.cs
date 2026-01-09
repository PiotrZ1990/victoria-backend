namespace Victoria.Web.Models.Cases;

public class CaseStageUpdateViewModel
{
    public int CaseFileId { get; set; }
    public string Stage { get; set; } = default!;
}
