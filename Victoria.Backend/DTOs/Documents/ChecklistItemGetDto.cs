namespace Victoria.Backend.DTOs.Documents;

public class ChecklistItemGetDto
{
    public int ItemId { get; set; }          // Id statusu (CaseApplicationChecklistItem / CaseVisaChecklistItem)
    public int ChecklistId { get; set; }     // Id szablonu
    public string DocumentType { get; set; }         // Nazwa z szablonu
    public bool IsRequired { get; set; }     // Czy wymagany
    public bool IsCompleted { get; set; }    // Status dla sprawy
    public int? DocumentId { get; set; }     // (na razie może być null)
    public DateTime UpdatedAt { get; set; }
}
