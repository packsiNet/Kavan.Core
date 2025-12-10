using ApplicationLayer.Dto.News;
using MediatR;

namespace ApplicationLayer.Features.News.Query;

public record GetNewsQuery(GetNewsRequestDto Model) : IRequest<HandlerResult>;
