namespace Victoria.Backend.DTOs.Documents;

public class ChecklistItemUpdateDto
{
    public bool IsCompleted { get; set; }
    public int? DocumentId { get; set; } // opcjonalnie, na razie może być null
}
