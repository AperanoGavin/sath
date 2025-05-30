using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using PRS.Domain.Entities;

namespace PRS.Infrastructure.EF.Configurations;

internal class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.HasKey(static r => r.Id);
        builder.Property(static r => r.Key).IsRequired();
        builder.Property(static r => r.Name).IsRequired();
    }
}