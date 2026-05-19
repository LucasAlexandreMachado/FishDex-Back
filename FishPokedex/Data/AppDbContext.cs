using FishPokedex.Models;
using Microsoft.EntityFrameworkCore;

namespace FishPokedex.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Species> Species { get; set; } = null!;
    public DbSet<Location> Locations { get; set; } = null!;
    public DbSet<Catch> Catches { get; set; } = null!;
    public DbSet<CatchDetail> CatchDetails { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1:N Relationship: Species -> Catch
        modelBuilder.Entity<Catch>()
            .HasOne(c => c.Species)
            .WithMany(s => s.Catches)
            .HasForeignKey(c => c.SpeciesId)
            .OnDelete(DeleteBehavior.Restrict);

        // 1:N Relationship: Location -> Catch
        modelBuilder.Entity<Catch>()
            .HasOne(c => c.Location)
            .WithMany(l => l.Catches)
            .HasForeignKey(c => c.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        // 1:1 Relationship: Catch -> CatchDetail
        modelBuilder.Entity<CatchDetail>()
            .HasOne(cd => cd.Catch)
            .WithOne(c => c.CatchDetail)
            .HasForeignKey<CatchDetail>(cd => cd.CatchId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique Index on CatchDetail.CatchId to enforce 1:1 constraint
        modelBuilder.Entity<CatchDetail>()
            .HasIndex(cd => cd.CatchId)
            .IsUnique();
    }
}
