using Driver.Services.Domain.AggregatesModel.DriverAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Driver.Services.Infrastructure.Persistence.Configurations;

public class DriverConfiguration : IEntityTypeConfiguration<Domain.AggregatesModel.DriverAggregate.Driver>
{
    public void Configure(EntityTypeBuilder<Domain.AggregatesModel.DriverAggregate.Driver> builder)
    {
        builder.ToTable("Drivers");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .ValueGeneratedNever();

        builder.Property(d => d.FullName)
            .IsRequired()
            .HasMaxLength(100);

        // Configure PhoneNumber as owned entity (value object)
        builder.OwnsOne(d => d.PhoneNumber, phone =>
        {
            phone.Property(p => p.Value)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("PhoneNumber");
        });

        builder.Property(d => d.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(d => d.VerificationStatus)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(50);

        // Configure VehicleInfo as owned entity (value object)
        builder.OwnsOne(d => d.VehicleInfo, vehicle =>
        {
            vehicle.Property(v => v.VehicleType)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("VehicleType");

            vehicle.Property(v => v.LicensePlate)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("LicensePlate");

            vehicle.Property(v => v.VehicleBrand)
                .HasMaxLength(50)
                .HasColumnName("VehicleBrand");

            vehicle.Property(v => v.VehicleModel)
                .HasMaxLength(50)
                .HasColumnName("VehicleModel");

            vehicle.Property(v => v.VehicleYear)
                .HasColumnName("VehicleYear");

            vehicle.Property(v => v.VehicleColor)
                .HasMaxLength(30)
                .HasColumnName("VehicleColor");
        });

        builder.Property(d => d.LicenseNumber)
            .HasMaxLength(50);

        builder.Property(d => d.ProfileImageUrl)
            .HasMaxLength(500);

        builder.Property(d => d.VerifiedAt);

        builder.Property(d => d.RejectionReason)
            .HasMaxLength(500);

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.UpdatedAt);

        // Indexes
        builder.HasIndex(d => d.Email)
            .IsUnique();

        builder.HasIndex(d => d.Status);

        builder.HasIndex(d => d.VerificationStatus);

        // Ignore domain events (not persisted)
        builder.Ignore(d => d.DomainEvents);
    }
}
