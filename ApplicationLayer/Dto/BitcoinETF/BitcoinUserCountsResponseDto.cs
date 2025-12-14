namespace ApplicationLayer.Dto.BitcoinETF;

public class BitcoinUserCountsResponseDto
{
    public List<BitcoinUserCountDailyDto> Daily { get; set; } = new();
    public List<BitcoinUserCountWeeklyDto> Weekly { get; set; } = new();
    public List<BitcoinUserCountMonthlyDto> Monthly { get; set; } = new();
}
