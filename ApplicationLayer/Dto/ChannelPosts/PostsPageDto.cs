using ApplicationLayer.Dto.BaseDtos;

namespace ApplicationLayer.DTOs.ChannelPosts;

public class PostsPageDto
{
    public List<PostDto> Items { get; set; } = [];
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}

public class GetPostsRequestDto
{
    public PaginationDto Pagination { get; set; } = new();
}

public class ReactPostDto
{
    public int PostId { get; set; }
    public string Reaction { get; set; }
}