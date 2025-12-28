using Victoria.Domain.Entities.Cases;

namespace Victoria.Domain.Entities.Documents;

public class CaseApplicationChecklistItem
{
    public int Id { get; set; }

    public int CaseFileId { get; set; }
    public CaseFile CaseFile { get; set; }

    // wskazuje na “szablon” pozycji checklisty (Twoja tabela ApplicationDocumentChecklist)
    public int ChecklistId { get; set; }
    public ApplicationDocumentChecklist Checklist { get; set; }

    public bool IsCompleted { get; set; } = false;

    // jeśli dokument został dosłany i zapisany w Documents -> tu go podepniemy
    public int? DocumentId { get; set; }
    public Document? Document { get; set; }

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
