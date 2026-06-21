using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SplitSpace.SpaceService.Domain.Models.Aggregates.Space;
using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Dal.ModelConfigurations;

internal class SpaceMemberConfiguration : IEntityTypeConfiguration<SpaceMember>
{
    public void Configure(EntityTypeBuilder<SpaceMember> builder)
    {
        builder.ToTable("space_membership");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnType("uuid")
            .HasConversion(id => id.Value, guid => new SpaceMemberId(guid));

        builder.Property(e => e.SpaceId)
            .HasColumnType("uuid")
            .HasConversion(id => id.Value, guid => new SpaceId(guid))
            .IsRequired();

        builder.Property(e => e.UserId)
            .HasColumnType("uuid")
            .HasConversion(id => id.Value, guid => new UserId(guid))
            .IsRequired();

        builder.Property(e => e.Role)
            .HasColumnName("role")
            .HasConversion<string>()
            .HasColumnType("text")
            .IsRequired();

        builder.Property(e => e.JoinedAt)
            .IsRequired();
    }
}