namespace Victoria.Web.Models.Documents;

public class ChecklistItemViewModel
{
    public int ItemId { get; set; }
    public string DocumentType { get; set; } = default!;
    public bool IsRequired { get; set; }
    public bool IsCompleted { get; set; }
}
