using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.DTOs.ChannelPosts;

public class CreateSignalPostDto
{
    public int ChannelId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public IFormFile Image { get; set; }
    public string Symbol { get; set; }
    public string Timeframe { get; set; }
    public int TradeType { get; set; }
    public List<decimal> EntryPoints { get; set; } = [];
    public List<decimal> Tps { get; set; } = [];
    public decimal StopLoss { get; set; }
}