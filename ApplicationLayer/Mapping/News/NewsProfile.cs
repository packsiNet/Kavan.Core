using ApplicationLayer.Dto.News;
using AutoMapper;
using DomainLayer.Entities;

namespace ApplicationLayer.Mapping.News;

public class NewsProfile : Profile
{
    public NewsProfile()
    {
        CreateMap<NewsInstrument, NewsInstrumentDto>();
        CreateMap<NewsPost, NewsPostDto>();
    }
}
