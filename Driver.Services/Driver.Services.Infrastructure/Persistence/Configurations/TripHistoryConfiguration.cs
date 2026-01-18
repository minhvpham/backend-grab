using Driver.Services.Domain.AggregatesModel.TripHistoryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Driver.Services.Infrastructure.Persistence.Configurations;

public class TripHistoryConfiguration : IEntityTypeConfiguration<TripHistory>
{
    public void Configure(EntityTypeBuilder<TripHistory> builder)
    {
        builder.ToTable("TripHistories");

        builder.HasKey(th => th.Id);

        builder.Property(th => th.Id)
            .ValueGeneratedNever();

        builder.Property(th => th.DriverId)
            .IsRequired();

        builder.HasOne(th => th.Driver)
            .WithMany()
            .HasForeignKey(th => th.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(th => th.OrderId)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(th => th.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // Location details
        builder.Property(th => th.PickupAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(th => th.PickupLatitude)
            .IsRequired();

        builder.Property(th => th.PickupLongitude)
            .IsRequired();

        builder.Property(th => th.DeliveryAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(th => th.DeliveryLatitude)
            .IsRequired();

        builder.Property(th => th.DeliveryLongitude)
            .IsRequired();

        // Distance and duration
        builder.Property(th => th.DistanceKm);

        builder.Property(th => th.DurationMinutes);

        // Financial
        builder.Property(th => th.Fare)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(th => th.CashCollected)
            .HasPrecision(18, 2);

        // Timestamps
        builder.Property(th => th.AssignedAt)
            .IsRequired();

        builder.Property(th => th.AcceptedAt);

        builder.Property(th => th.PickedUpAt);

        builder.Property(th => th.DeliveredAt);

        builder.Property(th => th.CancelledAt);

        // Additional info
        builder.Property(th => th.CancellationReason)
            .HasMaxLength(500);

        builder.Property(th => th.FailureReason)
            .HasMaxLength(500);

        builder.Property(th => th.CustomerNotes)
            .HasMaxLength(1000);

        builder.Property(th => th.DriverNotes)
            .HasMaxLength(1000);

        // Rating
        builder.Property(th => th.CustomerRating);

        builder.Property(th => th.CustomerFeedback)
            .HasMaxLength(1000);

        builder.Property(th => th.CreatedAt)
            .IsRequired();

        builder.Property(th => th.UpdatedAt);

        // Indexes
        builder.HasIndex(th => th.DriverId);

        builder.HasIndex(th => th.OrderId)
            .IsUnique();

        builder.HasIndex(th => th.Status);

        builder.HasIndex(th => th.AssignedAt);

        builder.HasIndex(th => th.DeliveredAt);

        // Ignore domain events
        builder.Ignore(th => th.DomainEvents);
    }
}
