using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.FinancialPeriod;
using MediatR;

namespace ApplicationLayer.Features.FinancialPeriod.Query;

public record GetActiveFinancialPeriodQuery : IRequest<HandlerResult>;
