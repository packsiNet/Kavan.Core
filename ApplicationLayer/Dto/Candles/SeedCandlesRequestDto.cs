namespace ApplicationLayer.Dto.Candles;

public class SeedCandlesRequestDto
{
    public int Count { get; set; } = 100;

    public DateTime? StartTime { get; set; }
}