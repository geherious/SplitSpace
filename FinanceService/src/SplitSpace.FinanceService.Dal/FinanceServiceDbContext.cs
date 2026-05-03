using Microsoft.EntityFrameworkCore;
using SplitSpace.FinanceService.Dal.Models.Entities;

namespace SplitSpace.FinanceService.Dal;

public class FinanceServiceDbContext : DbContext
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Debt> Debts => Set<Debt>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<ExpenseSplit> ExpenseSplits => Set<ExpenseSplit>();
    public DbSet<ExpenseTag> ExpenseTags => Set<ExpenseTag>();
    public DbSet<SpaceMembership> SpaceMemberships => Set<SpaceMembership>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Settlement> Settlements => Set<Settlement>();


    public FinanceServiceDbContext(DbContextOptions<FinanceServiceDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable("FINANCE_SERVICE_DB_CONNECTION_STRING")
            ?? "Host=localhost;Database=finance_db;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString);
        optionsBuilder.UseSnakeCaseNamingConvention();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("account");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Balance)
                .HasColumnName("balance")
                .HasPrecision(18, 2)
                .IsRequired();

            entity.Property(e => e.OwnerType)
                .HasColumnName("owner_type")
                .HasConversion<string>()
                .HasColumnType("text")
                .IsRequired();

            entity.HasIndex(e => e.OwnerId)
                .HasDatabaseName("idx_account_owner_id");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("category");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Limit)
                .HasColumnName("limit")
                .HasPrecision(18, 2);
            
            entity.HasOne<Category>()
                .WithMany()
                .HasForeignKey(e => e.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.SpaceId)
                .HasDatabaseName("idx_category_space_id");

            entity.HasIndex(e => e.ParentId)
                .HasDatabaseName("idx_category_parent_id");
        });

        modelBuilder.Entity<Debt>(entity =>
        {
            entity.ToTable("debt");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Amount)
                .HasColumnName("amount")
                .HasPrecision(18, 2)
                .IsRequired();

            entity.HasIndex(e => new { e.SpaceId, e.FromUserId, e.ToUserId })
                .IsUnique()
                .HasDatabaseName("idx_debt_space_from_to");
        });

        modelBuilder.Entity<Expense>(entity =>
        {
            entity.ToTable("expense");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Amount)
                .HasColumnName("amount")
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.HasOne<Category>()
                .WithMany()
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasOne<Account>()
                .WithMany()
                .HasForeignKey(e => e.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.SpaceId)
                .HasDatabaseName("idx_expense_space_id");

            entity.HasIndex(e => e.CreatedBy)
                .HasDatabaseName("idx_expense_created_by");

            entity.HasIndex(e => e.CategoryId)
                .HasDatabaseName("idx_expense_category_id");

            entity.HasIndex(e => e.AccountId)
                .HasDatabaseName("idx_expense_account_id");
        });

        modelBuilder.Entity<ExpenseSplit>(entity =>
        {
            entity.ToTable("expense_split");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.AmountToPay)
                .HasColumnName("amount_to_pay")
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.HasOne<Expense>()
                .WithMany()
                .HasForeignKey(e => e.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.SpaceId)
                .HasDatabaseName("idx_expense_split_space_id");

            entity.HasIndex(e => e.UserId)
                .HasDatabaseName("idx_expense_split_user_id");
            
            entity.HasIndex(e => new { e.ExpenseId, e.UserId })
                .IsUnique()
                .HasDatabaseName("idx_expense_split_expense_user");
        });

        modelBuilder.Entity<ExpenseTag>(entity =>
        {
            entity.ToTable("expense_tag");

            entity.HasKey(e => e.Id);
            
            entity.HasOne<Tag>()
                .WithMany()
                .HasForeignKey(e => e.TagId)
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasOne<Expense>()
                .WithMany()
                .HasForeignKey(e => e.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.ExpenseId)
                .HasDatabaseName("idx_expense_tag_expense_id");

            entity.HasIndex(e => e.TagId)
                .HasDatabaseName("idx_expense_tag_tag_id");
        });
        
        modelBuilder.Entity<SpaceMembership>(entity =>
        {
            entity.ToTable("space_membership");

            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => new { e.SpaceId, e.UserId })
                .IsUnique()
                .HasDatabaseName("idx_space_membership_space_user");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("tag");

            entity.HasKey(e => e.Id);

            entity.HasIndex(e => e.SpaceId)
                .HasDatabaseName("idx_tag_space_id");
        });
        
        modelBuilder.Entity<Settlement>(entity =>
        {
            entity.ToTable("settlement");

            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Amount)
                .HasColumnName("amount")
                .HasPrecision(18, 2)
                .IsRequired();
            
            entity.HasOne<Account>()
                .WithMany()
                .HasForeignKey(e => e.FromAccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
