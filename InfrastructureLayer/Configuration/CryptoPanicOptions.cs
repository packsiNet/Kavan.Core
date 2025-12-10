namespace InfrastructureLayer.Configuration;

public class CryptoPanicOptions
{
    public string BaseUrl { get; set; } = "https://cryptopanic.com/api/developer/v2";
    public string AuthToken { get; set; }
    public bool RunStartupSync { get; set; } = true;
}
