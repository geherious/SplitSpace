using Google.Type;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitSpace.ExternalFacade.Dal.ClientFacades;
using SplitSpace.ExternalFacade.Extensions;
using SplitSpace.ExternalFacade.Logic.Models.Requests.Finance;
using SplitSpace.ExternalFacade.Mappers;
using SplitSpace.FinanceService.Api.SearchService;

namespace SplitSpace.ExternalFacade.Controllers;

[ApiController]
[Authorize]
[Route("v1")]
public class SearchController : ControllerBase
{
    private readonly ISearchServiceClientFacades _searchServiceClientFacades;

    public SearchController(ISearchServiceClientFacades searchServiceClientFacades)
    {
        _searchServiceClientFacades = searchServiceClientFacades;
    }

    [HttpGet("spaces/{spaceId::guid}/analytics/category-report")]
    public async Task<IActionResult> GetReportAsync(
        Guid spaceId,
        [FromQuery] GetCategoryReportApiRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new 
            { 
                Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }
        
        var grpcRequest = new GetReportRequest
        {
            SpaceId = spaceId.ToString(),
            UserId = HttpContext.GetUserId().ToString(),
            From = new Date { Year = request.From.Year, Month = request.From.Month, Day = request.From.Day },
            To = new Date { Year = request.To.Year, Month = request.To.Month, Day = request.To.Day },
            Timezone = request.Timezone,
        };
        var grpcResponse = await _searchServiceClientFacades.GetReportAsync(grpcRequest, cancellationToken);
        
        return Ok(grpcResponse.ToApiResponse());
    }
}
