using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using MoviesService.Models;

namespace MoviesService.Data;

public class MoviesDbContext : DbContext
{
    public MoviesDbContext(DbContextOptions<MoviesDbContext> options) : base(options) { }

    public DbSet<Movie> Movies => Set<Movie>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Director).HasMaxLength(200);
            entity.Property(e => e.ProductionCompany).HasMaxLength(200);
            entity.Property(e => e.MainActors).HasMaxLength(1000);
            entity.Property(e => e.Description).HasMaxLength(5000);
            entity.Property(e => e.ImageUrl).HasMaxLength(1000);
            entity.Property(e => e.RelatedBookIds).HasConversion(
                v => string.Join(",", v),
                v => v.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
            ).Metadata.SetValueComparer(new ValueComparer<List<int>>(
                (c1, c2) => c1!.SequenceEqual(c2!),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            ));
        });
    }
}
