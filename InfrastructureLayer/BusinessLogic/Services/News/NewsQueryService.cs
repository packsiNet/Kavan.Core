using ApplicationLayer.Dto.BaseDtos;
using ApplicationLayer.Dto.News;
using ApplicationLayer.Interfaces;
using ApplicationLayer.Interfaces.Services.News;
using AutoMapper;
using DomainLayer.Common.Attributes;
using DomainLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureLayer.BusinessLogic.Services.News;

[InjectAsScoped]
public class NewsQueryService : INewsQueryService
{
    private readonly IRepository<NewsPost> _newsRepo;
    private readonly IMapper _mapper;

    public NewsQueryService(IRepository<NewsPost> newsRepo, IMapper mapper)
    {
        _newsRepo = newsRepo;
        _mapper = mapper;
    }

    public async Task<Result<NewsPageDto>> GetAsync(GetNewsRequestDto dto)
    {
        IQueryable<NewsPost> query = _newsRepo.Query();

        if (dto.Currencies.Length > 0)
        {
            query = query.Where(x => x.Instruments.Any(i => dto.Currencies.Contains(i.Code)));
        }

        if (dto.Regions.Length > 0)
        {
            query = query.Where(x => dto.Regions.Contains(x.SourceRegion));
        }

        if (!string.IsNullOrWhiteSpace(dto.Kind))
        {
            query = query.Where(x => x.Kind == dto.Kind);
        }

        if (!string.IsNullOrWhiteSpace(dto.Search))
        {
            var s = dto.Search;
            query = query.Where(x => x.Title.Contains(s) || x.Description.Contains(s));
        }

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(x => x.PublishedAt)
            .Include(x => x.Instruments)
            .Skip(dto.Pagination.Skip)
            .Take(dto.Pagination.PageSize)
            .ToListAsync();

        var dtoItems = _mapper.Map<NewsPostDto[]>(items);
        var page = new NewsPageDto
        {
            Items = dtoItems,
            Total = total,
            Page = dto.Pagination.Page,
            PageSize = dto.Pagination.PageSize
        };

        return Result<NewsPageDto>.Success(page);
    }
}
