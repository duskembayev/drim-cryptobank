using cryptobank.api.features.accounts.domain;
using cryptobank.api.features.news.domain;
using cryptobank.api.features.users.domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace cryptobank.api.db;

public class CryptoBankDbContext : DbContext
{
    public CryptoBankDbContext(DbContextOptions<CryptoBankDbContext> dbContextOptions)
        : base(dbContextOptions)
    {
    }

    public DbSet<News> News { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Role> Roles { get; set; } = null!;
    public DbSet<Account> Accounts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<News>(BuildNews)
            .Entity<User>(BuildUser)
            .Entity<Role>(BuildRole)
            .Entity<Account>(BuildAccount);
    }

    private static void BuildAccount(EntityTypeBuilder<Account> builder)
    {
        builder.ToTable("Account");
        builder.HasKey(a => a.AccountId);
        
        builder
            .Property(a => a.AccountId)
            .IsRequired()
            .HasMaxLength(36);
        
        builder
            .HasOne(a => a.User)
            .WithMany()
            .IsRequired();
        
        builder
            .Property(a => a.Currency)
            .IsRequired()
            .HasMaxLength(3);
        
        builder
            .Property(a => a.Balance)
            .IsRequired()
            .HasPrecision(18, 2);
        
        builder
            .Property(a => a.DateOfOpening)
            .IsRequired();
    }

    private static void BuildNews(EntityTypeBuilder<News> builder)
    {
        builder.ToTable("News");
        builder.HasKey(n => n.Id);

        builder
            .Property(n => n.Id)
            .IsRequired()
            .HasMaxLength(36);

        builder
            .Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder
            .Property(n => n.Content)
            .HasColumnType("text")
            .IsRequired();

        builder
            .Property(n => n.Date)
            .IsRequired();

        builder
            .Property(n => n.Author)
            .IsRequired()
            .HasMaxLength(100);
    }

    private static void BuildUser(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("User");
        builder.HasKey(u => u.Id);

        builder
            .Property(u => u.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder
            .Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(512);

        builder
            .Property(u => u.DateOfBirth)
            .HasColumnType("date")
            .IsRequired();

        builder
            .Property(u => u.DateOfRegistration)
            .IsRequired();

        builder
            .HasMany(u => u.Roles)
            .WithMany()
            .UsingEntity("UserRole");

        builder
            .HasMany(u => u.Accounts)
            .WithOne(a => a.User)
            .IsRequired();

        builder
            .HasIndex(u => u.Email)
            .IsUnique();
    }

    private static void BuildRole(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Role");
        builder.HasKey(r => r.Id);

        builder
            .Property(r => r.Id)
            .ValueGeneratedNever()
            .IsRequired();

        builder
            .Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);
    }
}