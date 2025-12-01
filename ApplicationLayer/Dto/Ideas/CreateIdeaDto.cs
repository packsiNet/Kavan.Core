using Microsoft.AspNetCore.Http;

namespace ApplicationLayer.DTOs.Ideas;

public class CreateIdeaDto
{
    public string Symbol { get; set; }

    public string Timeframe { get; set; }

    public int Trend { get; set; }

    public string Title { get; set; }

    public string Description { get; set; }

    public IFormFile Image { get; set; }

    public int Status { get; set; }

    public List<string> Tags { get; set; } = [];
}