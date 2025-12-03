using ApplicationLayer.Dto.AI;
using MediatR;

namespace ApplicationLayer.Features.Ai.Query;

public record TranslateTextQuery(AiTranslateRequestDto Model) : IRequest<AiTranslateResponseDto>;

