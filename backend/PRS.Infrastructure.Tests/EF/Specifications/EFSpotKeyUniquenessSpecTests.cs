using FluentAssertions;

using PRS.Domain.Entities;
using PRS.Infrastructure.EF.Contexts;
using PRS.Infrastructure.EF.Repositories;
using PRS.Infrastructure.EF.Specifications;
using PRS.Infrastructure.Tests.EF.Contexts;

namespace PRS.Infrastructure.Tests.EF.Specifications
{
    public class EFSpotKeyUniquenessSpecTests
    {
        private static AppDbContext CreateContext(string dbName) => InMemoryContextFactory.Create(dbName);

        [Fact]
        public async Task IsSatisfiedBy_WhenNoSpotWithKeyExists_ReturnsTrue()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            await using var ctx = CreateContext(dbName);

            // No spots in DB
            var spotRepo = new EFSpotRepository(ctx);
            var spec = new EFSpotKeyUniquenessSpec(spotRepo);

            // Act
            var result = await spec.IsSatisfiedBy("Z09", CancellationToken.None);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsSatisfiedBy_WhenSpotWithKeyExists_ReturnsFalse()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            await using var ctx = CreateContext(dbName);

            var spot = Spot.Create("XX1").Value;
            await ctx.Spots.AddAsync(spot);
            await ctx.SaveChangesAsync();

            var spotRepo = new EFSpotRepository(ctx);
            var spec = new EFSpotKeyUniquenessSpec(spotRepo);

            // Act
            var result = await spec.IsSatisfiedBy("XX1", CancellationToken.None);

            // Assert
            result.Should().BeFalse();
        }
    }
}