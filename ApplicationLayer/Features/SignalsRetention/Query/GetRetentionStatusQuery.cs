using ApplicationLayer.Dto.SignalsRetention;
using MediatR;

namespace ApplicationLayer.Features.SignalsRetention.Query
{
    public record GetRetentionStatusQuery() : IRequest<RetentionStatusDto>;
}