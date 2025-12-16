namespace Victoria.Backend.DTOs.Crm;

public class CaseFileGetDto
{
    public int Id { get; set; }
    public string CaseNumber { get; set; }
    public int LeadId { get; set; }
    public string Stage { get; set; }
    public string InternalNotes { get; set; }
    public DateTime CreatedAt { get; set; }
}
