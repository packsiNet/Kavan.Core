using MediatR;

namespace ApplicationLayer.Features.TimeFrames.Query;

public record GetTimeFramesQuery : IRequest<HandlerResult>;