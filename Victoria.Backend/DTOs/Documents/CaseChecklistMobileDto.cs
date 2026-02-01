namespace Victoria.Backend.DTOs.Documents;

public class CaseChecklistMobileDto
{
    public int CaseFileId { get; set; }

    // jeżeli sprawa ma study/visa - zwracamy ID, bo upload dokumentu w Twoim backendzie
    // jest przez StudyApplicationId/VisaApplicationId
    public int? StudyApplicationId { get; set; }
    public int? VisaApplicationId { get; set; }

    public List<ChecklistItemMobileDto> Items { get; set; } = new();
}

public class ChecklistItemMobileDto
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
