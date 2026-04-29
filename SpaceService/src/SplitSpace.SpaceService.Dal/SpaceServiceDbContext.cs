using Microsoft.EntityFrameworkCore;
using SplitSpace.SpaceService.Dal.Models.Entities;

namespace SplitSpace.SpaceService.Dal;

public class SpaceServiceDbContext : DbContext
{
    public DbSet<Space> Spaces => Set<Space>();
    public DbSet<SpaceMembership> SpaceMemberships => Set<SpaceMembership>();
    public DbSet<Invitation> Invitations => Set<Invitation>();

    public SpaceServiceDbContext(DbContextOptions<SpaceServiceDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable("SPACE_SERVICE_DB_CONNECTION_STRING")
            ?? "Host=localhost;Database=space_db;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.UseSnakeCaseNamingConvention();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Space entity configuration
        modelBuilder.Entity<Space>(entity =>
        {
            entity.ToTable("space");

            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Type)
                .HasColumnName("type")
                .HasConversion<string>()
                .HasColumnType("text")
                .IsRequired();
        });
        
        modelBuilder.Entity<SpaceMembership>(entity =>
        {
            entity.ToTable("space_membership");
            
            entity.HasKey(e => e.Id);
            
            
            entity.Property(e => e.Role)
                .HasColumnName("role")
                .HasConversion<string>()
                .HasColumnType("text")
                .IsRequired();
            
            // Foreign key
            entity.HasOne<Space>()
                .WithMany()
                .HasForeignKey(e => e.SpaceId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes and constraints
            entity.HasIndex(e => e.SpaceId)
                .HasDatabaseName("idx_space_membership_space_id");
            
            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("idx_space_membership_user_id");
            
            // Composite unique index to prevent duplicate memberships
            entity.HasIndex(e => new { e.SpaceId, e.UserId })
                .IsUnique()
                .HasDatabaseName("idx_space_membership_space_user_unique");
        });
        
        modelBuilder.Entity<Invitation>(entity =>
        {
            entity.ToTable("invitation");
            
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasConversion<string>()
                .HasColumnType("text")
                .IsRequired();
            
            // Foreign key
            entity.HasOne<Space>()
                .WithMany()
                .HasForeignKey(i => i.SpaceId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Indexes
            entity.HasIndex(e => e.SpaceId)
                .HasDatabaseName("idx_invitation_space_id");
            
            entity.HasIndex(e => e.InvitedUserId)
                .HasDatabaseName("idx_invitation_invited_user_id");
            
            // Composite index for finding active invitations
            entity.HasIndex(e => new { e.SpaceId, e.InvitedUserId, e.Status })
                .HasDatabaseName("idx_invitation_space_user_status");
        });
    }
}
