using System.Text;
using System.Text.Json;
using Victoria.Mobile.Helpers;
using Victoria.Mobile.Models.Auth;

namespace Victoria.Mobile.Services;

public class AuthService
{
    private static readonly JsonSerializerOptions JsonOpts = new() { PropertyNameCaseInsensitive = true };

    public async Task<string> LoginAsync(string email, string password)
    {
        using var http = new HttpClient { BaseAddress = new Uri(AppConfig.BaseUrl) };

        var payload = new LoginRequest { Email = email, Password = password };
        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        // ✅ jeśli u Ciebie backend ma inną ścieżkę - zmień tylko to:
        var resp = await http.PostAsync("api/auth/login", content);

        var body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
            throw new Exception($"Login failed {(int)resp.StatusCode}: {body}");

        var dto = JsonSerializer.Deserialize<LoginResponse>(body, JsonOpts);
        if (dto == null || string.IsNullOrWhiteSpace(dto.Token))
            throw new Exception("Login OK but token missing in response.");

        return dto.Token;
    }
    public async Task ChangePasswordAsync(string currentPassword, string newPassword)
    {
        var api = new ApiClient();

        var payload = new
        {
            currentPassword,
            newPassword
        };

        var json = JsonSerializer.Serialize(payload);

        // ApiClient ma PostJsonAsync, to wykorzystujemy:
        await api.PostJsonAsync("api/auth/change-password", json);
    }

}
