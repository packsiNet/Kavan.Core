namespace ApplicationLayer.Dto.External.Dune;

public class DuneEtfIssuerFlowRow
{
    public string time { get; set; }
    public string issuer { get; set; }
    public string etf_ticker { get; set; }
    public decimal amount { get; set; }
    public decimal amount_usd { get; set; }
    public decimal? amount_net_flow { get; set; }
    public decimal? amount_usd_net_flow { get; set; }
}
