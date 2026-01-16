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

    public async Task<string> GetStringAsync(string relativeUrl)
    {
        var resp = await _http.GetAsync(relativeUrl);
        var body = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
            throw new Exception($"API ERROR {(int)resp.StatusCode}: {body}");

        return body;
    }
}
