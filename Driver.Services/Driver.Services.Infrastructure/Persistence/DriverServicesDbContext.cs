using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using Driver.Services.Domain.AggregatesModel.DriverLocationAggregate;
using Driver.Services.Domain.AggregatesModel.DriverWalletAggregate;
using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using Microsoft.EntityFrameworkCore;

namespace Driver.Services.Infrastructure.Persistence;

public class DriverServicesDbContext : DbContext
{
    public DriverServicesDbContext(DbContextOptions<DriverServicesDbContext> options)
        : base(options)
    {
    }

    public DbSet<Domain.AggregatesModel.DriverAggregate.Driver> Drivers { get; set; }
    public DbSet<DriverLocation> DriverLocations { get; set; }
    public DbSet<DriverWallet> DriverWallets { get; set; }
    public DbSet<TripHistory> TripHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DriverServicesDbContext).Assembly);
    }
}
