using ApplicationLayer.Dto.News;
using MediatR;

namespace ApplicationLayer.Features.News.Commands;

public record SyncNewsCommand(CryptoPanicQuery Query) : IRequest<HandlerResult>;
