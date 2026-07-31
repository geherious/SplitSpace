using Npgsql;

namespace SplitSpace.SharedKernel.Database.Connections;

public interface IDbConnectionFactory
{
    Task<NpgsqlConnection> CreateAsync(CancellationToken ct = default);
}
