namespace Victoria.Backend.DTOs.Documents;

public class DocumentStatusUpdateDto
{
    public string Status { get; set; } = default!; // "Approved" / "Rejected" / "Uploaded"
}
