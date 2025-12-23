namespace DomainLayer.Entities;

public class TradeEmotion
{
    public int ConfidenceLevel { get; set; } // 1-5
    public string EmotionBeforeEntry { get; set; } = string.Empty;
    public bool PlanCompliance { get; set; }
}
