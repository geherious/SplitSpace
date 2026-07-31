using Dapper;
using Mediator;
using SplitSpace.SharedKernel.Models;
using SplitSpace.Spaces.Dal;
using SplitSpace.Spaces.Dal.Database.Connections;

namespace SplitSpace.Spaces.Logic.Features.Spaces.GetSpaces;

public class GetSpacesHandler : IQueryHandler<GetSpacesQuery, Result<GetSpacesResultData>>
{
    private readonly ISpaceDbConnectionFactory _connectionFactory;

    public GetSpacesHandler(ISpaceDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async ValueTask<Result<GetSpacesResultData>> Handle(GetSpacesQuery query, CancellationToken ct)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);

        var data = (await connection.QueryAsync<GetSpacesResultData.Space>(
            """
            SELECT
                s.id AS Id,
                s.name AS Name,
                s.type AS SpaceType,
                m.role AS Role
            FROM space s
            JOIN space_membership m ON s.id = m.space_id
            WHERE m.user_id = @UserId
            """,
            new { UserId = query.UserId.Value })).ToArray();

        return Result<GetSpacesResultData>.Success(new GetSpacesResultData(data));
    }
}
