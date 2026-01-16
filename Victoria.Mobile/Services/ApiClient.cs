using System.Net.Http.Headers;
using Victoria.Mobile.Helpers;

namespace Victoria.Mobile.Services;

public class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient()
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri(AppConfig.BaseUrl)
        };
    }

    private async Task ApplyAuthAsync()
    {
        var token = await TokenStore.GetAsync();
        if (!string.IsNullOrWhiteSpace(token))
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        else
            _http.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<string> GetStringAsync(string relativeUrl)
    {
        await ApplyAuthAsync();

        var resp = await _http.GetAsync(relativeUrl);
        var body = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
            throw new Exception($"API ERROR {(int)resp.StatusCode}: {body}");

        return body;
    }

    public async Task<string> PostJsonAsync(string relativeUrl, string json)
    {
        await ApplyAuthAsync();

        using var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var resp = await _http.PostAsync(relativeUrl, content);
        var body = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
            throw new Exception($"API ERROR {(int)resp.StatusCode}: {body}");

        return body;
    }
}
