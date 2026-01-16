namespace Victoria.Mobile.Config;

public static class AppConfig
{
    // Android emulator -> localhost komputera to 10.0.2.2
    // Jeśli backend stoi np. na https://localhost:7282, to emulator używa:
    public const string ApiBaseUrl_AndroidEmulator = "https://10.0.2.2:7282/";

    // Na Windows (np. test na Windows app)
    public const string ApiBaseUrl_Windows = "https://localhost:7282/";

    // Domyślnie ustawimy emulator (łatwiej na start)
    public static string ApiBaseUrl => ApiBaseUrl_AndroidEmulator;
}
