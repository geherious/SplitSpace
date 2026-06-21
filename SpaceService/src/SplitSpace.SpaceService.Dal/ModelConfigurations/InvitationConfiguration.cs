using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SplitSpace.SpaceService.Domain.Models.Aggregates.Invitation;
using SplitSpace.SpaceService.Domain.Models.Aggregates.Space;
using SplitSpace.SpaceService.Domain.Models.Ids;

namespace SplitSpace.SpaceService.Dal.ModelConfigurations;

public class InvitationConfiguration : IEntityTypeConfiguration<Invitation>
{
    public void Configure(EntityTypeBuilder<Invitation> builder)
    {
        builder.ToTable("invitation");
            
        // properties
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id)
            .HasColumnType("uuid")
            .HasConversion(id => id.Value, guid => new InvitationId(guid));
        
        builder.Property(e => e.SpaceId)
            .HasColumnType("uuid")
            .HasConversion(id => id.Value, guid => new SpaceId(guid));
        
        builder.Property(e => e.InvitedUserId)
            .HasColumnType("uuid")
            .HasConversion(id => id.Value, guid => new UserId(guid));
        
        builder.Property(e => e.InvitedBy)
            .HasColumnType("uuid")
            .HasConversion(id => id.Value, guid => new UserId(guid));
            
        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasColumnType("text")
            .IsRequired();
        
        builder.Property(e => e.ExpiresAt).IsRequired();
        builder.Property(e => e.CreatedAt).IsRequired();
            
        // Foreign keys
        builder.HasOne<Space>()
            .WithMany()
            .HasForeignKey(i => i.SpaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
