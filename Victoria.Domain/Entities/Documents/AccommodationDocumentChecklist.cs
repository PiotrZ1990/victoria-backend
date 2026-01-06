namespace Victoria.Domain.Entities.Documents;

public class AccommodationDocumentChecklist
{
    public int Id { get; set; }
    public string DocumentType { get; set; } = default!;
    public bool IsRequired { get; set; }
}
