using ApplicationLayer.Dto.SignalsCatalog;
using ApplicationLayer.Features.SignalsCatalog.Query;
using ApplicationLayer.Interfaces;
using DomainLayer.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ApplicationLayer.Features.SignalsCatalog.Handler
{
    public class GetSignalCatalogTreeHandler : IRequestHandler<GetSignalCatalogTreeQuery, List<SignalCatalogNodeDto>>
    {
        private readonly IRepository<SignalCatalogNode> _repo;

        public GetSignalCatalogTreeHandler(IRepository<SignalCatalogNode> repo)
        {
            _repo = repo;
        }

        public async Task<List<SignalCatalogNodeDto>> Handle(GetSignalCatalogTreeQuery request, CancellationToken cancellationToken)
        {
            var items = await _repo.Query().AsNoTracking().ToListAsync(cancellationToken);
            var byParent = items.GroupBy(x => x.ParentId).ToDictionary(g => g.Key, g => g.ToList());
            List<SignalCatalogNodeDto> Build(int? pid)
            {
                var list = new List<SignalCatalogNodeDto>();
                if (!byParent.ContainsKey(pid)) return list;
                foreach (var n in byParent[pid])
                {
                    var dto = new SignalCatalogNodeDto
                    {
                        Id = n.Id,
                        NameFa = n.NameFa,
                        NameEn = n.NameEn,
                        Kind = n.Kind,
                        Category = n.Category,
                        SignalName = n.SignalName,
                        Children = Build(n.Id)
                    };
                    list.Add(dto);
                }
                return list;
            }
            return Build(null);
        }
    }
}