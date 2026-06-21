using Microsoft.EntityFrameworkCore;
using SplitSpace.SpaceService.Dal.ModelConfigurations;
using SplitSpace.SpaceService.Domain.Models.Aggregates.Invitation;
using SplitSpace.SpaceService.Domain.Models.Aggregates.Space;

namespace SplitSpace.SpaceService.Dal;

public class SpaceServiceDbContext : DbContext
{
    public DbSet<Space> Spaces => Set<Space>();
    
    public DbSet<SpaceMember> SpaceMembers => Set<SpaceMember>();
    
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
        modelBuilder.ApplyConfiguration(new SpaceConfiguration());
        modelBuilder.ApplyConfiguration(new SpaceMemberConfiguration());
        modelBuilder.ApplyConfiguration(new InvitationConfiguration());
    }
}
