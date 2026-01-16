namespace Victoria.Mobile.Services;

public static class TokenStore
{
    private const string Key = "jwt_token";

    public static Task SaveAsync(string token) =>
        SecureStorage.SetAsync(Key, token);

    public static async Task<string?> GetAsync()
    {
        try
        {
            return await SecureStorage.GetAsync(Key);
        }
        catch
        {
            return null;
        }
    }

    public static void Clear()
    {
        try { SecureStorage.Remove(Key); } catch { /* ignore */ }
    }
}
