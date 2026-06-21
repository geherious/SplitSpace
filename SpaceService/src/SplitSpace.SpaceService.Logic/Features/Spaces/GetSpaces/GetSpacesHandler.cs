using Mediator;
using Microsoft.EntityFrameworkCore;
using SplitSpace.SpaceService.Common.Models;
using SplitSpace.SpaceService.Dal;

namespace SplitSpace.SpaceService.Logic.Features.Spaces.GetSpaces;

public class GetSpacesHandler : IQueryHandler<GetSpacesQuery, Result<GetSpacesResultData>>
{
    private readonly SpaceServiceDbContext _context;

    public GetSpacesHandler(SpaceServiceDbContext context)
    {
        _context = context;
    }

    public async ValueTask<Result<GetSpacesResultData>> Handle(GetSpacesQuery query, CancellationToken ct)
    {
        var data = await _context.Spaces
            .AsNoTracking()
            .Join(
                _context.SpaceMembers.AsNoTracking(),
                s => s.Id,
                m => m.SpaceId,
                (s, m) => new { s, m })
            .Where(x => x.m.UserId == query.UserId)
            .Select(x => new GetSpacesResultData.Space
            {
                Id = x.s.Id,
                Name = x.s.Name,
                SpaceType = x.s.Type,
                Role = x.m.Role
            })
            .ToArrayAsync(ct);

        return Result<GetSpacesResultData>.Success(new GetSpacesResultData(data));
    }
}
