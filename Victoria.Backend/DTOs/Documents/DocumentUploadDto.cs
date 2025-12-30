namespace Victoria.Backend.DTOs.Documents;

public class DocumentUploadDto
{
    public string DocumentType { get; set; } = default!;

    // podaj jedno z dwóch:
    public int? StudyApplicationId { get; set; }
    public int? VisaApplicationId { get; set; }
}
