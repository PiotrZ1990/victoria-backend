namespace Victoria.Mobile.Models;

public class CaseChecklistModel
{
    public int CaseFileId { get; set; }
    public int? StudyApplicationId { get; set; }
    public int? VisaApplicationId { get; set; }
    public List<ChecklistItemModel> Items { get; set; } = new();
}

public class ChecklistItemModel
{
    public string Flow { get; set; } = default!; // "Application" / "Visa"
    public int ItemId { get; set; }
    public int ChecklistId { get; set; }

    public string DocumentType { get; set; } = default!;
    public bool IsRequired { get; set; }

    public bool IsCompleted { get; set; }
    public int? DocumentId { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
