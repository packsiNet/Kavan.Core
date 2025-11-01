using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.TimeFrames;
using ApplicationLayer.Extensions;
using ApplicationLayer.Features.TimeFrames.Query;
using ApplicationLayer.Interfaces;
using AutoMapper;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Features.TimeFrames.Handler;

public class GetTimeFramesHandler(
    IRepository<TimeFrame> _timeFrameRepository,
    IMapper _mapper
) : IRequestHandler<GetTimeFramesQuery, HandlerResult>
{
    public async Task<HandlerResult> Handle(GetTimeFramesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var timeFrames = await _timeFrameRepository
                .Query()
                .Where(tf => tf.IsActive)
                .OrderBy(tf => tf.DisplayOrder)
                .ToListAsync(cancellationToken);

            var timeFrameDtos = _mapper.Map<List<TimeFrameDto>>(timeFrames);

            // Set categories based on duration
            foreach (var dto in timeFrameDtos)
            {
                dto.Category = GetTimeFrameCategory(dto.DurationInMinutes);
            }

            var result = Result<List<TimeFrameDto>>.Success(timeFrameDtos);
            return result.ToHandlerResult();
        }
        catch (Exception ex)
        {
            var result = Result<List<TimeFrameDto>>.GeneralFailure($"خطا در دریافت تایم فریم‌ها: {ex.Message}");
            return result.ToHandlerResult();
        }
    }

    private static string GetTimeFrameCategory(int durationInMinutes)
    {
        return durationInMinutes switch
        {
            <= 60 => "short",      // 1 hour or less
            <= 1440 => "medium",   // 1 day or less
            _ => "long"            // More than 1 day
        };
    }
}