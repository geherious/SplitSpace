namespace SplitSpace.FinanceService.Logic.Models.Results;

public record GetCategoriesResultData(IReadOnlyCollection<GetCategoriesResultData.Category> Categories)
{
    public record Category
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required Guid? ParentId { get; init; }
        public required decimal? Limit { get; init; }

    }
}
