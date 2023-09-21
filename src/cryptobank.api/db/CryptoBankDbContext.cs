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
    public DbSet<InternalTransfer> InternalTransfers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<News>(BuildNews)
            .Entity<User>(BuildUser)
            .Entity<Role>(BuildRole)
            .Entity<Account>(BuildAccount)
            .Entity<InternalTransfer>(BuildInternalTransfer);
    }

    private void BuildInternalTransfer(EntityTypeBuilder<InternalTransfer> builder)
    {
        builder.ToTable("InternalTransfer");
        builder.HasKey(t => t.Id);

        builder
            .Property(t => t.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.MoneyProperty(t => t.ConversionRate);

        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(t => t.SourceUserId)
            .IsRequired();

        builder
            .HasOne<Account>()
            .WithMany()
            .HasForeignKey(t => t.SourceAccountId)
            .IsRequired();

        builder.CurrencyProperty(t => t.SourceCurrency);
        builder.MoneyProperty(t => t.SourceAmount);

        builder
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(t => t.TargetUserId);

        builder
            .HasOne<Account>()
            .WithMany()
            .HasForeignKey(t => t.TargetAccountId)
            .IsRequired();

        builder.CurrencyProperty(t => t.TargetCurrency);
        builder.MoneyProperty(t => t.TargetAmount);

        builder
            .Property(t => t.Comment)
            .IsRequired()
            .HasMaxLength(500);
        
        builder
            .Property(t => t.DateOfCreation)
            .IsRequired();
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
            .WithMany(u => u.Accounts)
            .HasForeignKey(a => a.UserId)
            .IsRequired();

        builder.CurrencyProperty(a => a.Currency);
        builder.MoneyProperty(a => a.Balance);

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