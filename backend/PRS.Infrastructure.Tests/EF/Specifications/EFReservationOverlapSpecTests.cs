using FluentAssertions;

using PRS.Domain.Entities;
using PRS.Infrastructure.EF.Contexts;
using PRS.Infrastructure.EF.Repositories;
using PRS.Infrastructure.EF.Specifications;
using PRS.Infrastructure.Tests.EF.Contexts;

namespace PRS.Infrastructure.Tests.EF.Specifications
{
    public class EFReservationOverlapSpecTests
    {
        private static AppDbContext CreateContext(string dbName) => InMemoryContextFactory.Create(dbName);

        [Fact]
        public async Task IsSatisfiedBy_WhenNoReservationsForSpot_ReturnsTrue()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            await using var ctx = CreateContext(dbName);

            var spot = Spot.Create("O1").Value;
            var role = new UserRole("Employee", "Employee", "desc");
            var user = User.Create("Tony", "tony@ex.com", role).Value;

            await ctx.Spots.AddAsync(spot);
            await ctx.Roles.AddAsync(role);
            await ctx.Users.AddAsync(user);
            await ctx.SaveChangesAsync();

            var reservationRepo = new EFReservationRepository(ctx);
            var spec = new EFReservationOverlapSpec(reservationRepo);

            // Act
            var noOverlap = await spec.IsSatisfiedBy(spot, DateTime.Today.AddDays(1), DateTime.Today.AddDays(2), CancellationToken.None);

            // Assert
            noOverlap.Should().BeTrue();
        }

        [Fact]
        public async Task IsSatisfiedBy_WhenReservationExists_AndDatesDoNotOverlap_ReturnsTrue()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            await using var ctx = CreateContext(dbName);

            var spot = Spot.Create("O2").Value;
            var role = new UserRole("Employee", "Employee", "desc");
            var user = User.Create("Uma", "uma@ex.com", role).Value;

            await ctx.Spots.AddAsync(spot);
            await ctx.Roles.AddAsync(role);
            await ctx.Users.AddAsync(user);
            await ctx.SaveChangesAsync();

            var existingFrom = DateTime.Today.AddDays(2);
            var existingTo = DateTime.Today.AddDays(4);
            var existingRes = Reservation.Create(spot, user, existingFrom, existingTo).Value;
            await ctx.Reservations.AddAsync(existingRes);
            await ctx.SaveChangesAsync();

            var reservationRepo = new EFReservationRepository(ctx);
            var spec = new EFReservationOverlapSpec(reservationRepo);

            // Act
            var noOverlap = await spec.IsSatisfiedBy(spot, DateTime.Today.AddDays(4), DateTime.Today.AddDays(5), CancellationToken.None);

            // Assert
            noOverlap.Should().BeTrue();
        }

        [Fact]
        public async Task IsSatisfiedBy_WhenReservationExists_AndDatesOverlap_ReturnsFalse()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            await using var ctx = CreateContext(dbName);

            var spot = Spot.Create("O3").Value;
            var role = new UserRole("Employee", "Employee", "desc");
            var user = User.Create("Vera", "vera@ex.com", role).Value;

            await ctx.Spots.AddAsync(spot);
            await ctx.Roles.AddAsync(role);
            await ctx.Users.AddAsync(user);
            await ctx.SaveChangesAsync();

            var existingFrom = DateTime.Today.AddDays(5);
            var existingTo = DateTime.Today.AddDays(7);
            var existingRes = Reservation.Create(spot, user, existingFrom, existingTo).Value;
            await ctx.Reservations.AddAsync(existingRes);
            await ctx.SaveChangesAsync();

            var reservationRepo = new EFReservationRepository(ctx);
            var spec = new EFReservationOverlapSpec(reservationRepo);

            // Act
            var overlap = await spec.IsSatisfiedBy(spot, DateTime.Today.AddDays(6), DateTime.Today.AddDays(8), CancellationToken.None);

            // Assert
            overlap.Should().BeFalse();
        }
    }
}