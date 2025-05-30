using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using PRS.Domain.Entities;
using PRS.Domain.Enums;

namespace PRS.Infrastructure.EF.Configurations;

internal class SpotConfiguration : IEntityTypeConfiguration<Spot>
{
    public void Configure(EntityTypeBuilder<Spot> builder)
    {
        builder.HasKey(static s => s.Id);
        builder.Property(static s => s.Key).IsRequired();

        builder.HasIndex(static s => s.Key).IsUnique();

        var conv = new ValueConverter<List<SpotCapability>, string>(
            static v => string.Join(",", v.Select(static e => e.ToString())),
            static s => s.Split(",", StringSplitOptions.RemoveEmptyEntries)
                  .Select(static e => Enum.Parse<SpotCapability>(e))
                  .ToList()
        );
        builder.Property<List<SpotCapability>>("_caps")
         .HasConversion(conv);


        builder.HasMany(static s => s.Reservations)
           .WithOne(static r => r.Spot)
           .HasForeignKey("SpotId");


    }
}