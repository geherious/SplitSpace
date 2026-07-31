using Mediator;
using SplitSpace.SharedKernel.Models;

namespace SplitSpace.Finances.Logic.Features.Reports.GetReport;

public record GetReportCommand(
    Guid SpaceId,
    Guid UserId,
    DateTimeOffset DateFrom,
    DateTimeOffset DateTo)
    : IQuery<Result<GetReportResultData>>;
