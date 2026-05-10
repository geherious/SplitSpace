using Google.Type;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SplitSpace.ExternalFacade.Dal.ClientFacades;
using SplitSpace.ExternalFacade.Extensions;
using SplitSpace.ExternalFacade.Logic.Models.Requests.Finance;
using SplitSpace.ExternalFacade.Logic.Models.Responses.Finance;
using SplitSpace.ExternalFacade.Mappers;
using SplitSpace.FinanceService.Api.FinanceService;

namespace SplitSpace.ExternalFacade.Controllers;

[ApiController]
[Authorize]
[Route("v1")]
public class FinanceController : ControllerBase
{
    private readonly IFinanceServiceClientFacade _financeServiceClientFacade;

    public FinanceController(IFinanceServiceClientFacade financeServiceClientFacade)
    {
        _financeServiceClientFacade = financeServiceClientFacade;
    }

    [HttpPost("categories")]
    public async Task<IActionResult> AddCategoryAsync([FromBody] AddCategoryApiRequest request,
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
        var grpcResponse = await _financeServiceClientFacade.AddCategoryAsync(grpcRequest, cancellationToken);

        return Ok(new AddCategoryApiResponse { CategoryId = grpcResponse.CategoryId });
    }

    [HttpGet("spaces/{spaceId::guid}/categories")]
    public async Task<IActionResult> GetCategoriesAsync(Guid spaceId, CancellationToken cancellationToken)
    {
        var grpcRequest = new GetCategoriesRequest
        {
            SpaceId = spaceId.ToString(),
            UserId = HttpContext.GetUserId().ToString()
        };
        var grpcResponse = await _financeServiceClientFacade.GetCategoriesAsync(grpcRequest, cancellationToken);

        return Ok(grpcResponse.ToApiResponse());
    }

    [HttpPost("tags")]
    public async Task<IActionResult> AddTagAsync([FromBody] AddTagApiRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }

        var grpcRequest = new AddTagRequest
        {
            SpaceId = request.SpaceId.ToString(),
            UserId = HttpContext.GetUserId().ToString(),
            Name = request.Name
        };
        var grpcResponse = await _financeServiceClientFacade.AddTagAsync(grpcRequest, cancellationToken);

        return Ok(new AddTagApiResponse { TagId = grpcResponse.TagId });
    }

    [HttpGet("spaces/{spaceId::guid}/tags")]
    public async Task<IActionResult> GetTagsAsync(Guid spaceId, CancellationToken cancellationToken)
    {
        var grpcRequest = new GetTagsRequest
        {
            SpaceId = spaceId.ToString(),
            UserId = HttpContext.GetUserId().ToString()
        };
        var grpcResponse = await _financeServiceClientFacade.GetTagsAsync(grpcRequest, cancellationToken);

        return Ok(grpcResponse.ToApiResponse());
    }

    [HttpPost("spaces/{spaceId::guid}/accounts")]
    public async Task<IActionResult> AddSpaceAccountAsync(
        Guid spaceId,
        [FromBody] AddAccountApiRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }

        var grpcRequest = new AddAccountRequest
        {
            OwnerSpaceId = spaceId.ToString(),
            Name = request.Name,
            Balance = new Money { DecimalValue = request.Balance },
            ActorUserId = HttpContext.GetUserId().ToString()
        };
        var grpcResponse = await _financeServiceClientFacade.AddAccountAsync(grpcRequest, cancellationToken);

        return Ok(new AddAccountApiResponse { AccountId = grpcResponse.AccountId });
    }
    
    [HttpPost("me/accounts")]
    public async Task<IActionResult> AddPersonalAccountAsync(
        [FromBody] AddAccountApiRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }

        var grpcRequest = new AddAccountRequest
        {
            OwnerUserId = HttpContext.GetUserId().ToString(),
            Name = request.Name,
            Balance = new Money { DecimalValue = request.Balance },
            ActorUserId = HttpContext.GetUserId().ToString()
        };
        var grpcResponse = await _financeServiceClientFacade.AddAccountAsync(grpcRequest, cancellationToken);

        return Ok(new AddAccountApiResponse { AccountId = grpcResponse.AccountId });
    }

    [HttpGet("spaces/{spaceId::guid}/accounts")]
    public async Task<IActionResult> GetSpaceAccountsAsync(Guid spaceId, CancellationToken cancellationToken)
    {
        var grpcRequest = new GetSpaceAccountsRequest
        {
            SpaceId = spaceId.ToString(),
            UserId = HttpContext.GetUserId().ToString()
        };
        var grpcResponse = await _financeServiceClientFacade.GetSpaceAccountsAsync(grpcRequest, cancellationToken);

        return Ok(grpcResponse.ToApiResponse());
    }

    [HttpGet("me/accounts")]
    public async Task<IActionResult> GetPersonalAccountsAsync(CancellationToken cancellationToken)
    {
        var grpcRequest = new GetPersonalAccountsRequest
        {
            UserId = HttpContext.GetUserId().ToString()
        };
        var grpcResponse = await _financeServiceClientFacade.GetPersonalAccountsAsync(grpcRequest, cancellationToken);

        return Ok(grpcResponse.ToApiResponse());
    }

    [HttpPost("expenses")]
    public async Task<IActionResult> AddExpenseAsync([FromBody] AddExpenseApiRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new
            {
                Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
            });
        }

        if (request.Split is not null)
        {
            if (request.Split.Template is null)
            {
                return BadRequest("Split template is null");
            }
        }

        var grpcRequest = request.ToGrpcRequest(HttpContext.GetUserId());
        var grpcResponse = await _financeServiceClientFacade.AddExpenseAsync(grpcRequest, cancellationToken);

        return Ok(new AddExpenseApiResponse { ExpenseId = grpcResponse.ExpenseId });
    }

    [HttpGet("spaces/{spaceId::guid}/expenses")]
    public async Task<IActionResult> GetExpensesAsync(Guid spaceId, CancellationToken cancellationToken)
    {
        var grpcRequest = new GetExpensesRequest
        {
            SpaceId = spaceId.ToString(),
            UserId = HttpContext.GetUserId().ToString()
        };
        var grpcResponse = await _financeServiceClientFacade.GetExpensesAsync(grpcRequest, cancellationToken);

        return Ok(grpcResponse.ToApiResponse());
    }

    [HttpGet("spaces/{spaceId::guid}/debts")]
    public async Task<IActionResult> GetDebtsAsync(Guid spaceId, CancellationToken cancellationToken)
    {
        var grpcRequest = new GetDebtsRequest
        {
            SpaceId = spaceId.ToString(),
            UserId = HttpContext.GetUserId().ToString()
        };
        var grpcResponse = await _financeServiceClientFacade.GetDebtsAsync(grpcRequest, cancellationToken);

        return Ok(grpcResponse.ToApiResponse());
    }

    [HttpPost("settlements")]
    public async Task<IActionResult> SettleDebtAsync([FromBody] SettleDebtApiRequest request,
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
        await _financeServiceClientFacade.SettleDebtAsync(grpcRequest, cancellationToken);

        return Ok(new SettleDebtApiResponse());
    }
}
