namespace Victoria.Mobile.Helpers;

public static class AppConfig
{
    // Android Emulator -> localhost PC:
    // https://10.0.2.2:7282

    // Windows -> normalnie:
    // https://localhost:7282

    public static string BaseUrl
    {
        get
        {
#if ANDROID
            return "https://10.0.2.2:7282/";
#else
            return "https://localhost:7282/";
#endif
        }
    }
}
