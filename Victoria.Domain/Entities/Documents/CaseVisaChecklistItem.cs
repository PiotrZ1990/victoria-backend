using Victoria.Domain.Entities.Cases;

namespace Victoria.Domain.Entities.Documents;

public class CaseVisaChecklistItem
{
    public int Id { get; set; }

    public int CaseFileId { get; set; }
    public CaseFile CaseFile { get; set; }

    // wskazuje na “szablon” pozycji checklisty (Twoja tabela VisaDocumentChecklist)
    public int ChecklistId { get; set; }
    public VisaDocumentChecklist Checklist { get; set; }

    public bool IsCompleted { get; set; } = false;

    public int? DocumentId { get; set; }
    public Document? Document { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
