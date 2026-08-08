using SplitSpace.Finances.Domain.Models.Events;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.SharedKernel.Domain.Models;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Domain.Models.Aggregates.CategoryAggregate;

public sealed record Category : AggregateRoot<CategoryId>
{
    public override CategoryId Id { get; protected set; }

    public SpaceId SpaceId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public Guid? ParentId { get; private set; }

    public decimal? Limit { get; private set; }

    private Category(CategoryId id,
        SpaceId spaceId,
        string name,
        Guid? parentId,
        decimal? limit)
    {
        Id = id;
        SpaceId = spaceId;
        Name = name;
        ParentId = parentId;
        Limit = limit;
    }

    private Category() { }
    
    public static Category Rehydrate(CategoryId id,
        SpaceId spaceId,
        string name,
        Guid? parentId,
        decimal? limit)
    {
        return new Category
        {
            Id = id,
            SpaceId = spaceId,
            Name = name,
            ParentId = parentId,
            Limit = limit,
        };
    }

    public static Result<Category> Create(
        SpaceId spaceId,
        string name,
        Category? parent,
        decimal? limit)
    {
        if (parent?.ParentId is not null)
        {
            return Result<Category>.Failure(new Error(ErrorType.FailedPrecondition, "Parent category already has parent."));
        }

        var category = new Category(
            CategoryId.New(),
            spaceId,
            name,
            parent?.ParentId,
            limit);

        category.AddDomainEvent(new CategoryCreatedEvent(category));

        return Result.Success(category);
    }
}
