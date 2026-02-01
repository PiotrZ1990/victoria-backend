using System.Text.Json;
using Victoria.Mobile.Models;

namespace Victoria.Mobile.Services;

public class EnrollmentsService
{
    private readonly ApiClient _api;
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public EnrollmentsService(ApiClient api)
    {
        _api = api;
    }

    public async Task<List<EnrollmentListItemModel>> GetMyAsync()
    {
        var json = await _api.GetStringAsync("api/enrollments/my");
        return JsonSerializer.Deserialize<List<EnrollmentListItemModel>>(json, JsonOpts) ?? new();
    }

    public async Task EnrollMyAsync(int courseGroupId)
    {
        var payload = JsonSerializer.Serialize(new { courseGroupId });
        _ = await _api.PostJsonAsync("api/enrollments/my", payload);
    }
    public Task UnenrollMyAsync(int enrollmentId)
       => _api.DeleteAsync($"api/enrollments/my/{enrollmentId}");
}
