using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.ChannelPosts;

namespace ApplicationLayer.Interfaces.Channels;

public interface IChannelPostService
{
    Task<Result<PostDto>> CreateSignalAsync(CreateSignalPostDto dto);
    Task<Result<PostDto>> CreateNewsAsync(CreateNewsPostDto dto);
    Task<Result<PostDto>> UpdateSignalAsync(int postId, UpdateSignalPostDto dto);
    Task<Result<PostDto>> UpdateNewsAsync(int postId, UpdateNewsPostDto dto);
    Task<Result> DeleteAsync(int postId);
    Task<Result<PostsPageDto>> GetByChannelAsync(int channelId, GetPostsRequestDto dto);
    Task<Result<PostDto>> ReactAsync(ReactPostDto dto);
}