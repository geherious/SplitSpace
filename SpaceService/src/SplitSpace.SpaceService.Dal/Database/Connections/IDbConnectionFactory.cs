using Npgsql;

namespace SplitSpace.SpaceService.Dal.Database.Connections;

public interface IDbConnectionFactory
{
    Task<NpgsqlConnection> CreateAsync(CancellationToken ct = default);
}
