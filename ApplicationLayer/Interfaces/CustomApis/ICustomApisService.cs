using ApplicationLayer.Dto.BaseDtos;

namespace ApplicationLayer.Interfaces.CustomApis;

public interface ICustomApisService
{
    Task<Result> IngestAsync(DateTime StartDateUtc);
}