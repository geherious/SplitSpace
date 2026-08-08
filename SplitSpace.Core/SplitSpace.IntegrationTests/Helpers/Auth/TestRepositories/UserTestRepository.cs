using Dapper;
using SplitSpace.Auth.Dal.Database.Connections;
using SplitSpace.Auth.Dal.Database.Entities;
using SplitSpace.Auth.Domain.Models.Aggregates.UserAggregate;
using SplitSpace.Auth.Domain.Models.Ids;
using SplitSpace.Auth.Domain.Models.ValueObjects;

namespace SplitSpace.IntegrationTests.Helpers.Auth.TestRepositories;

public sealed class UserTestRepository
{
    private readonly IUserDomainRepository _domainRepository;
    private readonly IAuthDbConnectionFactory _connectionFactory;

    public UserTestRepository(
        IUserDomainRepository domainRepository,
        IAuthDbConnectionFactory connectionFactory)
    {
        _domainRepository = domainRepository;
        _connectionFactory = connectionFactory;
    }

    public Task SaveAsync(User user, CancellationToken ct = default)
    {
        return _domainRepository.SaveAsync(user, ct);
    }

    public Task<User?> GetAsync(UserId userId, CancellationToken ct = default)
    {
        return _domainRepository.GetAsync(userId, ct);
    }

    public Task<User?> GetAsync(Email email, CancellationToken ct = default)
    {
        return _domainRepository.GetAsync(email, ct);
    }

    public async Task<UserEntity?> ReadAsync(UserId userId, CancellationToken ct = default)
    {
        const string sql =
            """
            SELECT
                id,
                email,
                password_hash AS PasswordHash,
                created_at AS CreatedAt,
                last_login AS LastLogin
            FROM "user"
            WHERE id = @Id
            """;

        await using var connection = await _connectionFactory.CreateAsync(ct);
        var entity = (await connection.QueryAsync<UserEntity>(
            sql,
            new { Id = userId.Value })).SingleOrDefault();
        return entity;
    }
}
