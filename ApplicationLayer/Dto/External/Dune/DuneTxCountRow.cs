namespace ApplicationLayer.Dto.External.Dune;

public class DuneTxCountRow
{
    public string time { get; set; }
    public decimal tx_count { get; set; }
    public decimal tx_count_moving_average { get; set; }
}
