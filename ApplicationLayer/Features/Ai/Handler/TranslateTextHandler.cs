using ApplicationLayer.Dto.AI;
using ApplicationLayer.Features.Ai.Query;
using ApplicationLayer.Interfaces.External;
using MediatR;

namespace ApplicationLayer.Features.Ai.Handler;

public class TranslateTextHandler : IRequestHandler<TranslateTextQuery, AiTranslateResponseDto>
{
    private readonly IAiTranslationService _service;

    public TranslateTextHandler(IAiTranslationService service)
    {
        _service = service;
    }

    public async Task<AiTranslateResponseDto> Handle(TranslateTextQuery request, CancellationToken cancellationToken)
    {
        var translation = await _service.TranslateAsync(request.Model.Text, cancellationToken);
        return new AiTranslateResponseDto { Translation = translation };
    }
}

