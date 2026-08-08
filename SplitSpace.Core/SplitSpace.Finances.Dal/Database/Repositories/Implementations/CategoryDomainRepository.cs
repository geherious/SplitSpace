using Dapper;
using Npgsql;
using SplitSpace.Finances.Dal.Database.Connections;
using SplitSpace.Finances.Dal.Database.Entities;
using SplitSpace.Finances.Domain.Models.Aggregates.CategoryAggregate;
using SplitSpace.Finances.Domain.Models.Events;
using SplitSpace.Finances.Domain.Models.Ids;

namespace SplitSpace.Finances.Dal.Database.Repositories.Implementations;

public class CategoryDomainRepository : ICategoryDomainRepository
{
    private readonly IFinanceDbConnectionFactory _connectionFactory;
    private readonly NpgsqlTransaction? _transaction;

    public CategoryDomainRepository(IFinanceDbConnectionFactory connectionFactory)
        : this(connectionFactory, null)
    {
    }

    internal CategoryDomainRepository(IFinanceDbConnectionFactory connectionFactory, NpgsqlTransaction? transaction)
    {
        _connectionFactory = connectionFactory;
        _transaction = transaction;
    }

    public async Task SaveAsync(Category category, CancellationToken ct)
    {
        if (_transaction is not null)
        {
            await ApplyEventsAsync(_transaction, category);
            return;
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        await using var transaction = await connection.BeginTransactionAsync(ct);
        try
        {
            await ApplyEventsAsync(transaction, category);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    private static async Task ApplyEventsAsync(NpgsqlTransaction transaction, Category category)
    {
        foreach (var domainEvent in category.DomainEvents)
        {
            switch (domainEvent)
            {
                case CategoryCreatedEvent e:
                    await OnCategoryCreatedAsync(transaction, e);
                    break;
            }
        }
        
        category.ClearDomainEvents();
    }

    private static async Task OnCategoryCreatedAsync(NpgsqlTransaction transaction, CategoryCreatedEvent e)
    {
        const string sql =
            """
            INSERT INTO category (id, space_id, name, parent_id, "limit")
            VALUES (@Id, @SpaceId, @Name, @ParentId, @Limit)
            """;

        var parameters = new
        {
            Id = e.Category.Id.Value,
            SpaceId = e.Category.SpaceId.Value,
            e.Category.Name,
            e.Category.ParentId,
            e.Category.Limit
        };

        await transaction.Connection!.ExecuteAsync(sql, parameters, transaction);
    }

    public async Task<Category?> GetAsync(CategoryId categoryId, CancellationToken ct)
    {
        const string sql =
            """
            SELECT
                id,
                space_id AS SpaceId,
                name,
                parent_id AS ParentId,
                "limit"
            FROM category
            WHERE id = @Id
            """;

        if (_transaction is not null)
        {
            var entity = await _transaction.Connection!.QuerySingleOrDefaultAsync<CategoryEntity>(
                sql, new { Id = categoryId.Value }, _transaction);
            return entity is null ? null : Map(entity);
        }

        await using var connection = await _connectionFactory.CreateAsync(ct);
        var result = await connection.QuerySingleOrDefaultAsync<CategoryEntity>(sql, new { Id = categoryId.Value });
        return result is null ? null : Map(result);
    }

    private static Category Map(CategoryEntity entity)
    {
        return Category.Rehydrate(
            new CategoryId(entity.Id),
            new SpaceId(entity.SpaceId),
            entity.Name,
            entity.ParentId,
            entity.Limit);
    }
}
