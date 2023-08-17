using cryptobank.api.dal.news;
using Microsoft.EntityFrameworkCore;

namespace cryptobank.api.dal;

public class CryptoBankDbContext : DbContext
{
    public CryptoBankDbContext(DbContextOptions<CryptoBankDbContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    public DbSet<News> News { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var newsBuilder = modelBuilder.Entity<News>().ToTable("News");

        newsBuilder.HasKey(n => n.Id);
        
        newsBuilder
            .Property(n => n.Id)
            .IsRequired()
            .HasMaxLength(36);

        newsBuilder.Property(n => n.Title)
            .IsRequired()
            .HasMaxLength(500);

        newsBuilder.Property(n => n.Content)
            .HasColumnType("text")
            .IsRequired();

        newsBuilder.Property(n => n.Date)
            .HasColumnType("timestamp")
            .IsRequired();

        newsBuilder.Property(n => n.Author)
            .IsRequired()
            .HasMaxLength(100);
    }
}