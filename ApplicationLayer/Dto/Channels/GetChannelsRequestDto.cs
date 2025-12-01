using ApplicationLayer.Dto.BaseDtos;

namespace ApplicationLayer.DTOs.Channels;

public class GetChannelsRequestDto
{
    public int? Category { get; set; }
    public int? AccessType { get; set; }
    public string OwnerUsername { get; set; }
    public PaginationDto Pagination { get; set; } = new();
}