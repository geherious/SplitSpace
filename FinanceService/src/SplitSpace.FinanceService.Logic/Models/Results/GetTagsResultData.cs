namespace SplitSpace.FinanceService.Logic.Models.Results;

public record GetTagsResultData(IReadOnlyCollection<GetTagsResultData.Tag> Tags)
{
    public record Tag
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
    }
}
