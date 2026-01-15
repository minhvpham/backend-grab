using Driver.Services.Domain.AggregatesModel.DriverLocationAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Driver.Services.Infrastructure.Persistence.Configurations;

public class DriverLocationConfiguration : IEntityTypeConfiguration<DriverLocation>
{
    public void Configure(EntityTypeBuilder<DriverLocation> builder)
    {
        builder.ToTable("DriverLocations");

        builder.HasKey(dl => dl.Id);

        builder.Property(dl => dl.Id)
            .ValueGeneratedNever();

        builder.Property(dl => dl.DriverId)
            .IsRequired();

        builder.Property(dl => dl.Latitude)
            .IsRequired();

        builder.Property(dl => dl.Longitude)
            .IsRequired();

        builder.Property(dl => dl.Accuracy);

        builder.Property(dl => dl.Heading);

        builder.Property(dl => dl.Speed);

        builder.Property(dl => dl.Timestamp)
            .IsRequired();

        builder.Property(dl => dl.CreatedAt)
            .IsRequired();

        builder.Property(dl => dl.UpdatedAt);

        // Indexes
        builder.HasIndex(dl => dl.DriverId)
            .IsUnique(); // One location per driver

        builder.HasIndex(dl => dl.Timestamp);

        // Spatial index for location queries (if supported by database)
        builder.HasIndex(dl => new { dl.Latitude, dl.Longitude });

        // Ignore domain events
        builder.Ignore(dl => dl.DomainEvents);
    }
}
