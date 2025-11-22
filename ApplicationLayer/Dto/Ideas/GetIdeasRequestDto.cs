using ApplicationLayer.Dto.BaseDtos;

namespace ApplicationLayer.DTOs.Ideas;

public class GetIdeasRequestDto
{
    public string Symbol { get; set; }
    public string Timeframe { get; set; }
    public string Trend { get; set; }
    public List<string> Tags { get; set; } = [];
    public PaginationDto Pagination { get; set; } = new();
}