using System.ComponentModel.DataAnnotations;

namespace Victoria.Backend.DTOs.Crm;

public class CaseFileCreateFromLeadDto
{
    [Required]
    public int LeadId { get; set; }

    [MaxLength(500)]
    public string? InternalNotes { get; set; }
}
