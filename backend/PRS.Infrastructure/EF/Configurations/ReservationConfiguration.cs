using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PRS.Domain.Entities;
using PRS.Domain.Enums;

namespace PRS.Infrastructure.EF.Configurations;

internal class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(static r => r.Id);

        builder.Property(static r => r.From).IsRequired();
        builder.Property(static r => r.To).IsRequired();
        builder.Property<ReservationStatus>("Status")
               .HasConversion<string>();


        builder.Property<Guid>("SpotId");
        builder.HasOne(static r => r.Spot)
               .WithMany("Reservations")
               .HasForeignKey("SpotId");


        builder.Property<Guid>("UserId");
        builder.HasOne(static r => r.User)
               .WithMany()
               .HasForeignKey("UserId");
    }
}