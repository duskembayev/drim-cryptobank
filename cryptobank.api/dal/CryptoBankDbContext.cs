using cryptobank.api.dal.news;
using Microsoft.EntityFrameworkCore;

namespace cryptobank.api.dal;

public class CryptoBankDbContext : DbContext
{
    public CryptoBankDbContext(DbContextOptions<CryptoBankDbContext> dbContextOptions) : base(dbContextOptions)
    {
    }

    public DbSet<NewsModel> News { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var newsBuilder = modelBuilder.Entity<NewsModel>().ToTable("News");

        newsBuilder.HasKey(n => n.Mrn);
        
        newsBuilder
            .Property(n => n.Mrn)
            .HasColumnType("varchar(36)")
            .IsRequired()
            .HasMaxLength(36);

        newsBuilder.Property(n => n.Title)
            .HasColumnType("varchar(500)")
            .IsRequired()
            .HasMaxLength(500);

        newsBuilder.Property(n => n.Content)
            .HasColumnType("varchar(10000)")
            .IsRequired()
            .HasMaxLength(10000);

        newsBuilder.Property(n => n.Date)
            .HasColumnType("timestamp")
            .IsRequired();

        newsBuilder.Property(n => n.Author)
            .HasColumnType("varchar(100)")
            .IsRequired()
            .HasMaxLength(100);
    }
}