using Microsoft.EntityFrameworkCore;
using SplitSpace.AuthService.Dal.Models.Entities;

namespace SplitSpace.AuthService.Dal;

public class AuthServiceDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public AuthServiceDbContext(DbContextOptions<AuthServiceDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable("AUTH_SERVICE_DB_CONNECTION_STRING")
            ?? "Host=localhost;Database=auth_db;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.UseSnakeCaseNamingConvention();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User entity configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("user");

            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => e.Email)
                .IsUnique();
        });

        // RefreshToken entity configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("refresh_token");

            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => e.Token)
                .IsUnique();

            entity.HasOne<User>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.UserId);
        });
    }
}
