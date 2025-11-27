using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.DTOs.Channels;

namespace ApplicationLayer.Interfaces.Channels;

public interface IChannelService
{
    Task<Result<ChannelDto>> CreateAsync(CreateChannelDto dto);
    Task<Result<ChannelDto>> UpdateAsync(int id, UpdateChannelDto dto);
    Task<Result> DeleteAsync(int id);
    Task<Result<ChannelDto>> GetByIdAsync(int id);
    Task<Result<ChannelsPageDto>> GetMyAsync(GetChannelsRequestDto dto);
    Task<Result<ChannelsPageDto>> GetPublicAsync(GetChannelsRequestDto dto);
    Task<Result> JoinAsync(int channelId);
    Task<Result> LeaveAsync(int channelId);
    Task<Result<ChannelDto>> RateAsync(RateChannelDto dto);
}