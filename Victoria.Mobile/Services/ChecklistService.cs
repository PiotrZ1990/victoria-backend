using Victoria.Mobile.Models;

namespace Victoria.Mobile.Services;

public class ChecklistService
{
    private readonly ApiClient _api;

    public ChecklistService(ApiClient api)
    {
        _api = api;
    }

    public Task<CaseChecklistModel> GetMyChecklistAsync(int caseFileId)
        => _api.GetJsonAsync<CaseChecklistModel>($"api/casefiles/my/{caseFileId}/checklist");
}
