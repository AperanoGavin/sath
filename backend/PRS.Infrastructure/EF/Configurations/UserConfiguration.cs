using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PRS.Domain.Entities;

namespace PRS.Infrastructure.EF.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(static u => u.Id);
        builder.Property(static u => u.Name).IsRequired();
        builder.Property(static u => u.Email).IsRequired();

        builder.Property<Guid>("RoleId");
        builder.HasOne(static u => u.Role)
         .WithMany()
         .HasForeignKey("RoleId");
    }
}