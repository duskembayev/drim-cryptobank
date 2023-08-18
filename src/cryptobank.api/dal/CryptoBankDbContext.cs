using cryptobank.api.dal.news;
using cryptobank.api.dal.users;
using Microsoft.EntityFrameworkCore;

namespace cryptobank.api.dal;

public class CryptoBankDbContext : DbContext
{
    public CryptoBankDbContext(DbContextOptions<CryptoBankDbContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    public DbSet<News> News { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    internal DbSet<Role> Roles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var newsBuilder = modelBuilder.Entity<News>().ToTable("News");

        newsBuilder.HasKey(n => n.Id);

        newsBuilder
            .Property(n => n.Id)
            .IsRequired()
            .HasMaxLength(36);

        newsBuilder
            .Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(500);

        newsBuilder
            .Property(n => n.Content)
            .HasColumnType("text")
            .IsRequired();

        newsBuilder
            .Property(n => n.Date)
            .HasColumnType("timestamp")
            .IsRequired();

        newsBuilder
            .Property(n => n.Author)
            .IsRequired()
            .HasMaxLength(100);

        var userBuilder = modelBuilder.Entity<User>().ToTable("User");

        userBuilder.HasKey(u => u.Id);

        userBuilder
            .Property(u => u.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        userBuilder
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200);

        userBuilder
            .Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(256);

        userBuilder
            .Property(u => u.PasswordSalt)
            .IsRequired()
            .HasMaxLength(256);

        userBuilder
            .Property(u => u.DateOfBirth)
            .HasColumnType("date")
            .IsRequired();

        userBuilder
            .Property(u => u.DateOfRegistration)
            .IsRequired();

        userBuilder
            .Ignore(u => u.Role)
            .HasMany(u => u.Roles)
            .WithMany()
            .UsingEntity("UserRole");

        userBuilder
            .HasIndex(u => u.Email)
            .IsUnique();

        var roleBuilder = modelBuilder.Entity<Role>().ToTable("Role");

        roleBuilder.HasKey(r => r.Id);

        roleBuilder
            .Property(r => r.Id)
            .ValueGeneratedNever()
            .IsRequired();

        roleBuilder
            .Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);
    }
}