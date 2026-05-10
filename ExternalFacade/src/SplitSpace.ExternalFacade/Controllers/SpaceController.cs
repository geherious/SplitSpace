using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitSpace.ExternalFacade.Dal.ClientFacades;
using SplitSpace.ExternalFacade.Extensions;
using SplitSpace.ExternalFacade.Logic.Models.Requests.Space;
using SplitSpace.ExternalFacade.Mappers;
using SplitSpace.SpaceService.Api;

namespace SplitSpace.ExternalFacade.Controllers;

[ApiController]
[Authorize]
[Route("v1/spaces")]
public class SpaceController : ControllerBase
{
    private readonly ISpaceServiceClientFacade _spaceServiceClientFacade;

    public SpaceController(ISpaceServiceClientFacade spaceServiceClientFacade)
    {
        _spaceServiceClientFacade = spaceServiceClientFacade;
    }

    [HttpGet]
    public async Task<IActionResult> GetSpacesAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new 
            { 
                Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }

        var grpcRequest = new GetSpacesRequest { UserId = HttpContext.GetUserId().ToString() };
        var grpcResponse = await _spaceServiceClientFacade.GetSpacesAsync(grpcRequest, cancellationToken);
        
        return Ok(grpcResponse.ToApiResponse());
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateSpaceAsync([FromBody] CreateSpaceApiRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new 
            { 
                Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }

        var grpcRequest = request.ToGrpcRequest(HttpContext.GetUserId());
        var grpcResponse = await _spaceServiceClientFacade.CreateSpaceAsync(grpcRequest, cancellationToken);
        
        return Ok(grpcResponse.ToApiResponse());
    }
    
    [HttpDelete("{spaceId:guid}")]
    public async Task<IActionResult> CreateSpaceAsync(Guid spaceId, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new 
            { 
                Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }

        var grpcRequest = new DeleteSpaceRequest
        {
            SpaceId = spaceId.ToString(),
            UserId = HttpContext.GetUserId().ToString()
        };
        
        await _spaceServiceClientFacade.DeleteSpaceAsync(grpcRequest, cancellationToken);
        
        return Ok();
    }
}
