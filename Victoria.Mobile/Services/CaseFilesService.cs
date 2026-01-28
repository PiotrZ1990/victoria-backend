using System.Text.Json;
using Victoria.Mobile.Models;

namespace Victoria.Mobile.Services;

public class CaseFilesService
{
    private readonly ApiClient _api;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public CaseFilesService(ApiClient api)
    {
        _api = api;
    }

    // LISTA (u Ciebie już działa – zostawiamy)
    public async Task<List<CaseDetailsModel>> GetMyCasesAsync()
    {
        // U Ciebie to już działa, więc NIE zmieniamy endpointu.
        // Zakładam, że bierzesz listę z api/casefiles
        // Jeśli u Ciebie jest inne, to tu masz swój działający adres.
        return await _api.GetJsonAsync<List<CaseDetailsModel>>("api/casefiles");
    }

    public async Task<List<CaseDetailsModel>> GetAllAsync()
    {
        // lista case'ów (tak jak było wcześniej w projekcie)
        // endpoint ma być taki jak ten, z którego już korzystałeś i działał
        return await _api.GetJsonAsync<List<CaseDetailsModel>>("api/casefiles");
    }


    // DETAILS
    public async Task<CaseDetailsModel> GetByIdAsync(int id)
    {
        // Standardowo: GET api/casefiles/{id}
        return await _api.GetJsonAsync<CaseDetailsModel>($"api/casefiles/{id}");
    }
}
