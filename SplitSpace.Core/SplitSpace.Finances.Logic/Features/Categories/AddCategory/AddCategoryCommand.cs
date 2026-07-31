using Mediator;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Categories.AddCategory;

public record AddCategoryCommand(
    SpaceId SpaceId,
    UserId UserId,
    string Name,
    CategoryId? ParentId,
    decimal? Limit)
    : ICommand<Result<AddCategoryResultData>>;
