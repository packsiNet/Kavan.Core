namespace ApplicationLayer.Dto.BitcoinETF;

public class BitcoinEtfIssuerFlowDto
{
    public DateTime Time { get; set; }
    public string Issuer { get; set; } = string.Empty;
    public string EtfTicker { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal AmountUsd { get; set; }
    public decimal? AmountNetFlow { get; set; }
    public decimal? AmountUsdNetFlow { get; set; }
}
