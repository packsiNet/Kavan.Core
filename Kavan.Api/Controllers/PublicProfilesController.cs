using ApplicationLayer.Common.Enums;
using ApplicationLayer.Common.Extensions;
using ApplicationLayer.DTOs.Profiles.Organizations;
using ApplicationLayer.Features.Profiles.Organizations.Query;
using ApplicationLayer.Features.Profiles.Public.Query;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kavan.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[ApiExplorerSettings(GroupName = nameof(ApiDefinitions.Public))]
public class PublicProfilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PublicProfilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{userId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPublicAsync(int userId)
        => await ResultHelper.GetResultAsync(_mediator, new GetPublicProfileByUserIdQuery(userId));

    [HttpGet("{userId:int}/full")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFullPublicAsync(int userId)
        => await ResultHelper.GetResultAsync(_mediator, new GetUserPublicProfileQuery(userId));

    [HttpGet("organizations/search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchAsync([FromQuery] string country, [FromQuery] bool? hasExchange, [FromQuery] bool? hasInvestmentPanel,
                                                 [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var model = new SearchOrganizationsDto { Country = country, HasExchange = hasExchange, HasInvestmentPanel = hasInvestmentPanel, Page = page, PageSize = pageSize };
        return await ResultHelper.GetResultAsync(_mediator, new SearchOrganizationsQuery(model));
    }
}
