namespace Victoria.Backend.DTOs.Cases;

public class CaseReadinessDto
{
    public int CaseFileId { get; set; }
    public string Stage { get; set; }

    public int RequiredApplicationItems { get; set; }
    public int CompletedApplicationItems { get; set; }

    public int RequiredVisaItems { get; set; }
    public int CompletedVisaItems { get; set; }

    public int RequiredAccommodationItems { get; set; }
    public int CompletedAccommodationItems { get; set; }

    public bool CanMoveToApplication { get; set; }
    public bool CanMoveToVisa { get; set; }
    public bool CanMoveToAccommodation { get; set; }
    public bool CanComplete { get; set; }
}
