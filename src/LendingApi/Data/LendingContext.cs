using LendingApi.Models;
using Microsoft.EntityFrameworkCore;

namespace LendingApi.Data;

public class LendingContext : DbContext
{
    public LendingContext(DbContextOptions<LendingContext> options) : base(options) { }

    public DbSet<Item> Items => Set<Item>();
    public DbSet<Indicator> Indicators => Set<Indicator>();
    public DbSet<ItemIndicator> ItemIndicators => Set<ItemIndicator>();
    public DbSet<IncompatibleIndicator> IncompatibleIndicators => Set<IncompatibleIndicator>();
    public DbSet<LoanRequest> LoanRequests => Set<LoanRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemIndicator>()
            .HasKey(ii => new { ii.ItemId, ii.IndicatorId });

        modelBuilder.Entity<ItemIndicator>()
            .HasOne(ii => ii.Item)
            .WithMany(i => i.Indicators)
            .HasForeignKey(ii => ii.ItemId);

        modelBuilder.Entity<IncompatibleIndicator>()
            .HasIndex(x => new { x.IndicatorAId, x.IndicatorBId });
    }
}
