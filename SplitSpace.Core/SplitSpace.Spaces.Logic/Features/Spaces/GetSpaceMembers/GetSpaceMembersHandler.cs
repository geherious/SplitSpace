using Dapper;
using Mediator;
using SplitSpace.SharedKernel.Models;
using SplitSpace.Spaces.Dal;
using SplitSpace.Spaces.Dal.Database.Connections;

namespace SplitSpace.Spaces.Logic.Features.Spaces.GetSpaceMembers;

public class GetSpaceMembersHandler : IQueryHandler<GetSpaceMembersQuery, Result<GetSpaceMembersResultData>>
{
    private readonly ISpaceDbConnectionFactory _connectionFactory;

    public GetSpaceMembersHandler(ISpaceDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async ValueTask<Result<GetSpaceMembersResultData>> Handle(GetSpaceMembersQuery query, CancellationToken ct)
    {
        await using var connection = await _connectionFactory.CreateAsync(ct);

        var data = (await connection.QueryAsync<GetSpaceMembersResultData.SpaceMember>(
            """
            SELECT
                m.user_id AS UserId
            FROM space_membership m
            WHERE m.space_id = @SpaceId
            """,
            new { SpaceId = query.SpaceId.Value })).ToArray();

        return Result<GetSpaceMembersResultData>.Success(new GetSpaceMembersResultData(data));
    }
}
