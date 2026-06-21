using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SplitSpace.SpaceService.Domain.Models.Aggregates.Space;
using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Dal.ModelConfigurations;

internal class SpaceConfiguration : IEntityTypeConfiguration<Space>
{
    public void Configure(EntityTypeBuilder<Space> builder)
    {
        builder.ToTable("space");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnType("uuid")
            .HasConversion(id => id.Value, guid => new SpaceId(guid));

        builder.Property(e => e.Name)
            .IsRequired();

        builder.Property(e => e.Type)
            .HasConversion<string>()
            .HasColumnType("text")
            .IsRequired();

        builder.Property(e => e.OwnerId)
            .HasColumnType("uuid")
            .HasConversion(id => id.Value, guid => new UserId(guid));

        builder.HasMany(s => s.Members)
            .WithOne()
            .HasForeignKey(m => m.SpaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
