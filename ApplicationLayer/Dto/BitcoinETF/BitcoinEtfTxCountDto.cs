namespace ApplicationLayer.Dto.BitcoinETF;

public class BitcoinEtfTxCountDto
{
    public DateTime Time { get; set; }
    public decimal TxCount { get; set; }
    public decimal TxCountMovingAverage { get; set; }
}
