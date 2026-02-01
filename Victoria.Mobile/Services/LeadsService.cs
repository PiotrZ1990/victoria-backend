using System.Text.Json;
using Victoria.Mobile.Models;

namespace Victoria.Mobile.Services;

public class LeadsService
{
    private readonly ApiClient _api;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public LeadsService(ApiClient api)
    {
        _api = api;
    }

    public async Task CreateLeadAsync(LeadCreateRequest req)
    {
        var json = JsonSerializer.Serialize(new
        {
            fullName = req.FullName,
            email = req.Email,
            phoneNumber = req.PhoneNumber,
            source = req.Source,
            interestedCountry = req.InterestedCountry,
            interestedService = req.InterestedService
        });

        // ✅ backend: POST api/leads (AllowAnonymous)
        await _api.PostJsonAsync("api/leads", json);
    }
}
