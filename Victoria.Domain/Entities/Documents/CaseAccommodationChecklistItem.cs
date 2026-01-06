namespace Victoria.Domain.Entities.Documents;

public class CaseAccommodationChecklistItem
{
    public int Id { get; set; }

    public int CaseFileId { get; set; }        // bez nawigacji – jak w innych itemach
    public int ChecklistId { get; set; }
    public AccommodationDocumentChecklist Checklist { get; set; } = default!;

    public int? DocumentId { get; set; }       // opcjonalnie – jeśli chcesz link do Documents
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
}
