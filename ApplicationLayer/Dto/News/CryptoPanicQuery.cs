namespace ApplicationLayer.Dto.News;

public class CryptoPanicQuery
{
    public bool Public { get; set; } = true;

    public string[] Currencies { get; set; } = Array.Empty<string>();

    public string[] Regions { get; set; } = Array.Empty<string>();

    public string Filter { get; set; }

    public string Kind { get; set; }

    public bool? Following { get; set; }

    public string Search { get; set; }
}
