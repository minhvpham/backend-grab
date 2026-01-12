using Driver.Services.Domain.AggregatesModel.DriverWalletAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Driver.Services.Infrastructure.Persistence.Configurations;

public class DriverWalletConfiguration : IEntityTypeConfiguration<DriverWallet>
{
    public void Configure(EntityTypeBuilder<DriverWallet> builder)
    {
        builder.ToTable("DriverWallets");

        builder.HasKey(dw => dw.Id);

        builder.Property(dw => dw.Id)
            .ValueGeneratedNever();

        builder.Property(dw => dw.DriverId)
            .IsRequired();

        builder.Property(dw => dw.Balance)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(dw => dw.CashOnHand)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(dw => dw.TotalEarnings)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(dw => dw.TotalWithdrawn)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(dw => dw.LastWithdrawalAt);

        builder.Property(dw => dw.IsActive)
            .IsRequired();

        builder.Property(dw => dw.CreatedAt)
            .IsRequired();

        builder.Property(dw => dw.UpdatedAt);

        // Configure Transaction as owned collection
        builder.OwnsMany(dw => dw.Transactions, transaction =>
        {
            transaction.ToTable("Transactions");

            transaction.WithOwner()
                .HasForeignKey("WalletId");

            transaction.HasKey(t => t.Id);

            transaction.Property(t => t.Id)
                .ValueGeneratedNever();

            transaction.Property(t => t.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            transaction.Property(t => t.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            transaction.Property(t => t.BalanceBefore)
                .IsRequired()
                .HasPrecision(18, 2);

            transaction.Property(t => t.BalanceAfter)
                .IsRequired()
                .HasPrecision(18, 2);

            transaction.Property(t => t.OrderId)
                .HasMaxLength(100);

            transaction.Property(t => t.Reference)
                .HasMaxLength(200);

            transaction.Property(t => t.Description)
                .HasMaxLength(500);

            transaction.Property(t => t.CreatedAt)
                .IsRequired();

            // Indexes for transactions
            transaction.HasIndex(t => t.Type);
            transaction.HasIndex(t => t.CreatedAt);
            transaction.HasIndex(t => t.OrderId);
        });

        // Indexes
        builder.HasIndex(dw => dw.DriverId)
            .IsUnique();

        builder.HasIndex(dw => dw.IsActive);

        // Ignore domain events
        builder.Ignore(dw => dw.DomainEvents);
    }
}
