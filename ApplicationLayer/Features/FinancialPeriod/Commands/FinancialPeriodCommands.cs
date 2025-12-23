using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.FinancialPeriod;
using MediatR;

namespace ApplicationLayer.Features.FinancialPeriod.Commands;

public record CreateFinancialPeriodCommand(CreateFinancialPeriodDto Model) : IRequest<HandlerResult>;

public record CloseFinancialPeriodCommand(int Id) : IRequest<HandlerResult>;
