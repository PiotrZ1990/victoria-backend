namespace Victoria.Mobile.Models.Auth;

public class LoginResponse
{
    // dopasowane do typowych odpowiedzi; jeśli backend ma inne pola,
    // zmienisz nazwy (CaseInsensitive w JSON i tak pomoże).
    public string Token { get; set; } = "";
}
