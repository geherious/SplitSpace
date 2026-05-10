using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitSpace.ExternalFacade.Dal.ClientFacades;
using SplitSpace.ExternalFacade.Extensions;
using SplitSpace.ExternalFacade.Logic.Models.Requests.Space;
using SplitSpace.ExternalFacade.Logic.Models.Responses.Space;
using SplitSpace.ExternalFacade.Mappers;
using SplitSpace.SpaceService.Api;

namespace SplitSpace.ExternalFacade.Controllers;

[ApiController]
[Authorize]
[Route("v1/spaces/invitations")]
public class InvitationController : ControllerBase
{
    private readonly IInvitationServiceClientFacade _invitationServiceClientFacade;

    public InvitationController(IInvitationServiceClientFacade invitationServiceClientFacade)
    {
        _invitationServiceClientFacade = invitationServiceClientFacade;
    }
    
    [HttpPost]
    public async Task<IActionResult> InviteUserAsync([FromBody] InviteUserApiRequest request,
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
        var grpcResponse = await _invitationServiceClientFacade.InviteUserAsync(grpcRequest, cancellationToken);
        
        return Ok(new InviteUserApiResponse { InvitationId = grpcResponse.InvitationId });
    }
    
    [HttpPost("accept")]
    public async Task<IActionResult> AcceptInvitationAsync([FromBody] AcceptInvitationApiRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new 
            { 
                Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }

        var grpcRequest = new AcceptInvitationRequest
        {
            InvitationId = request.InvitationId.ToString(),
            UserId = HttpContext.GetUserId().ToString()
        };
        await _invitationServiceClientFacade.AcceptInvitationAsync(grpcRequest, cancellationToken);
        
        return Ok();
    }
    
    [HttpPost("reject")]
    public async Task<IActionResult> RejectInvitationAsync([FromBody] RejectInvitationApiRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new 
            { 
                Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }

        var grpcRequest = new RejectInvitationRequest
        {
            InvitationId = request.InvitationId.ToString(),
            UserId = HttpContext.GetUserId().ToString()
        };
        await _invitationServiceClientFacade.RejectInvitationAsync(grpcRequest, cancellationToken);
        
        return Ok();
    }
}
