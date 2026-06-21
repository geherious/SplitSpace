namespace SplitSpace.SpaceService.Domain.Models.Ids;

public readonly record struct InvitationId(Guid Value)
{
    public static InvitationId New() => new(Guid.CreateVersion7());
}
