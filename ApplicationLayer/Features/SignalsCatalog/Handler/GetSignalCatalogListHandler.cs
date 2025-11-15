using ApplicationLayer.Dto.SignalsCatalog;
using ApplicationLayer.Features.SignalsCatalog.Query;
using ApplicationLayer.Interfaces;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Features.SignalsCatalog.Handler
{
    public class GetSignalCatalogListHandler : IRequestHandler<GetSignalCatalogListQuery, List<SignalCatalogListItemDto>>
    {
        private readonly IRepository<SignalCatalogNode> _repo;

        public GetSignalCatalogListHandler(IRepository<SignalCatalogNode> repo)
        {
            _repo = repo;
        }

        public async Task<List<SignalCatalogListItemDto>> Handle(GetSignalCatalogListQuery request, CancellationToken cancellationToken)
        {
            return await _repo.Query()
                .AsNoTracking()
                .Where(x => x.Kind == "Signal")
                .Select(x => new SignalCatalogListItemDto
                {
                    Id = x.Id,
                    NameFa = x.NameFa,
                    NameEn = x.NameEn,
                    Category = x.Category,
                    SignalName = x.SignalName
                })
                .ToListAsync(cancellationToken);
        }
    }
}