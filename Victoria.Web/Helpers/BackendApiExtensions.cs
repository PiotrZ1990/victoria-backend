using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

namespace Victoria.Web.Helpers;

public static class BackendApiExtensions
{
    public static HttpClient WithJwt(this HttpClient client, ISession session)
    {
        var jwt = session.GetString("JWT");
        if (!string.IsNullOrWhiteSpace(jwt))
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

        return client;
    }
}
