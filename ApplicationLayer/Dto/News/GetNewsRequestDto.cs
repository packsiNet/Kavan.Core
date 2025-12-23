using ApplicationLayer.Dto.BaseDtos;

namespace ApplicationLayer.Dto.News;

public class GetNewsRequestDto
{
    public string[] Currencies { get; set; } = [];
    public string[] Regions { get; set; } = [];
    public string Kind { get; set; }
    public string Search { get; set; }
    public PaginationDto Pagination { get; set; } = new();
}
