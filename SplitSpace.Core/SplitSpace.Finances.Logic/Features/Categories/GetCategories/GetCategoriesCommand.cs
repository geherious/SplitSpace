using Mediator;
using SplitSpace.Finances.Domain.Models.Ids;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Categories.GetCategories;

public record GetCategoriesCommand(SpaceId SpaceId, UserId UserId)
    : IQuery<Result<GetCategoriesResultData>>;
