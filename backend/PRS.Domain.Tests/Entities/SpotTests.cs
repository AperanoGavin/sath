using FluentAssertions;

using PRS.Domain.Entities;
using PRS.Domain.Enums;
using PRS.Domain.Tests.Stubs;

namespace PRS.Domain.Tests.Entities
{
    public class SpotTests
    {
        private readonly UserRole _employeeRole =
            new("Employee", "Employee", "Regular employee");
        private readonly UserRole _managerRole =
            new("Manager", "Manager", "Parking manager");

        private readonly User _employee;
        private readonly User _manager;

        public SpotTests()
        {
            _employee = User.Create("Alice", "alice@acme.com", _employeeRole).Value;
            _manager = User.Create("Bob", "bob@acme.com", _managerRole).Value;
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_InvalidKey_ReturnsFailure(string invalidKey)
        {
            var result = Spot.Create(invalidKey);

            result.IsFailure.Should().BeTrue();
            result.Error!.Code.Should().Be("Spot.InvalidKey");
        }

        [Fact]
        public void Create_ValidKey_Succeeds()
        {
            var result = Spot.Create("A01");

            result.IsSuccess.Should().BeTrue();
            result.Value.Key.Should().Be("A01");
            result.Value.Capabilities.Should().BeEmpty();
            result.Value.Reservations.Should().BeEmpty();
        }

        [Fact]
        public void AddCapability_Duplicate_ReturnsFailure()
        {
            var spot = Spot.Create("B01").Value;
            spot.AddCapability(SpotCapability.ElectricCharger).IsSuccess.Should().BeTrue();

            var second = spot.AddCapability(SpotCapability.ElectricCharger);

            second.IsFailure.Should().BeTrue();
            second.Error!.Code.Should().Be("Spot.DuplicateCapability");
        }

        [Fact]
        public void AddCapability_New_Succeeds()
        {
            var spot = Spot.Create("C01").Value;

            var result = spot.AddCapability(SpotCapability.ElectricCharger);

            result.IsSuccess.Should().BeTrue();
            spot.Capabilities.Should().ContainSingle()
                .Which.Should().Be(SpotCapability.ElectricCharger);
        }

        [Fact]
        public void RemoveCapability_NotAssigned_Fails()
        {
            var spot = Spot.Create("D01").Value;

            var result = spot.RemoveCapability(SpotCapability.ElectricCharger);

            result.IsFailure.Should().BeTrue();
            result.Error!.Code.Should().Be("Spot.DoesNotHaveCapability");
        }

        [Fact]
        public void RemoveCapability_Existing_Succeeds()
        {
            var spot = Spot.Create("E01").Value;
            spot.AddCapability(SpotCapability.ElectricCharger).IsSuccess.Should().BeTrue();

            var result = spot.RemoveCapability(SpotCapability.ElectricCharger);

            result.IsSuccess.Should().BeTrue();
            spot.Capabilities.Should().BeEmpty();
        }

        [Fact]
        public async Task ReserveAsync_ValidPeriodNoOverlap_Succeeds()
        {
            var spot = Spot.Create("F01").Value;
            var from = DateTime.UtcNow.Date.AddDays(1);
            var to = from.AddDays(2);
            var spec = new FakeOverlapSpec(allowed: true);

            var result = await spot.ReserveAsync(_employee, from, to, spec);

            result.IsSuccess.Should().BeTrue();
            var r = result.Value;
            r.Spot.Should().BeSameAs(spot);
            r.User.Should().BeSameAs(_employee);
            r.From.Should().Be(from);
            r.To.Should().Be(to);
            spot.Reservations.Should().Contain(r);
        }

        [Fact]
        public async Task ReserveAsync_PastFrom_Fails()
        {
            var spot = Spot.Create("A02").Value;
            var from = DateTime.UtcNow.Date.AddDays(-1);
            var to = from.AddDays(1);
            var spec = new FakeOverlapSpec(allowed: true);

            var result = await spot.ReserveAsync(_employee, from, to, spec);

            result.IsFailure.Should().BeTrue();
            result.Error!.Code.Should().Be("Reservation.PastFrom");
        }

        [Fact]
        public async Task ReserveAsync_ChargerRequiredButNone_Fails()
        {
            var spot = Spot.Create("X01").Value;
            var from = DateTime.UtcNow.Date.AddDays(1);
            var to = from.AddDays(1);
            var spec = new FakeOverlapSpec(allowed: true);

            var result = await spot.ReserveAsync(
                user: _employee,
                from: from,
                to: to,
                needsCharger: true,
                overlapSpec: spec);

            result.IsFailure.Should().BeTrue();
            result.Error!.Code.Should().Be("Reservation.ChargerRequired");
        }

        [Fact]
        public async Task ReserveAsync_ChargerRequiredAndAvailable_Succeeds()
        {
            var spot = Spot.Create("Y01").Value;
            spot.AddCapability(SpotCapability.ElectricCharger).IsSuccess.Should().BeTrue();

            var from = DateTime.UtcNow.Date.AddDays(1);
            var to = from.AddDays(1);
            var spec = new FakeOverlapSpec(allowed: true);

            var result = await spot.ReserveAsync(
                user: _employee,
                from: from,
                to: to,
                needsCharger: true,
                overlapSpec: spec);

            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task ReserveAsync_ExceedsMaxDaysEmployee_Fails()
        {
            var spot = Spot.Create("B02").Value;
            var from = DateTime.UtcNow.Date;
            var to = from.AddDays(8);
            var spec = new FakeOverlapSpec(allowed: true);

            var result = await spot.ReserveAsync(_employee, from, to, spec);

            result.IsFailure.Should().BeTrue();
            result.Error!.Code.Should().Be("Reservation.TooLong");
        }

        [Fact]
        public async Task ReserveAsync_ManagerCanBook30Days_Succeeds()
        {
            var spot = Spot.Create("C02").Value;
            var from = DateTime.UtcNow.Date;
            var to = from.AddDays(30);
            var spec = new FakeOverlapSpec(allowed: true);

            var result = await spot.ReserveAsync(_manager, from, to, spec);

            result.IsSuccess.Should().BeTrue();
            result.Value.To.Should().Be(to);
        }

        [Fact]
        public async Task ReserveAsync_OverlapSpecFails_Fails()
        {
            var spot = Spot.Create("D02").Value;
            var from = DateTime.UtcNow.Date;
            var to = from.AddDays(1);
            var spec = new FakeOverlapSpec(allowed: false);

            var result = await spot.ReserveAsync(_employee, from, to, spec);

            result.IsFailure.Should().BeTrue();
            result.Error!.Code.Should().Be("Reservation.Overlap");
        }

        [Fact]
        public async Task CheckIn_Then_Cancel_TransitionsStateCorrectly()
        {
            var spot = Spot.Create("E02").Value;
            var from = DateTime.UtcNow.Date;
            var to = from.AddDays(1);
            var spec = new FakeOverlapSpec(allowed: true);

            var resResult = await spot.ReserveAsync(_employee, from, to, spec);
            var r = resResult.Value;

            // check-in before 11 AM should succeed
            var at = from.AddHours(10);
            var ci = spot.CheckIn(r.Id, at);
            ci.IsSuccess.Should().BeTrue();
            r.Status.Should().Be(ReservationStatus.CheckedIn);

            // cancelling a checked-in reservation is invalid
            var cancel = spot.Cancel(r.Id);
            cancel.IsFailure.Should().BeTrue();
            cancel.Error!.Code.Should().Be("Reservation.InvalidState");
        }

        [Fact]
        public async Task Cancel_Then_CancelAgain_Fails()
        {
            var spot = Spot.Create("F02").Value;
            var from = DateTime.UtcNow.Date;
            var to = from.AddDays(1);
            var spec = new FakeOverlapSpec(allowed: true);

            var r = (await spot.ReserveAsync(_employee, from, to, spec)).Value;

            var first = spot.Cancel(r.Id);
            first.IsSuccess.Should().BeTrue();
            r.Status.Should().Be(ReservationStatus.Cancelled);

            var second = spot.Cancel(r.Id);
            second.IsFailure.Should().BeTrue();
            second.Error!.Code.Should().Be("Reservation.AlreadyCancelled");
        }

        [Fact]
        public async Task ExpireNoShows_After11AM_ExpiresReservation()
        {
            var spot = Spot.Create("A03").Value;
            var from = DateTime.UtcNow.Date;
            var to = from.AddDays(1);
            var spec = new FakeOverlapSpec(allowed: true);

            var r = (await spot.ReserveAsync(_employee, from, to, spec)).Value;
            r.Status.Should().Be(ReservationStatus.Reserved);

            // simulate time = 12:00 on the 'from' date
            var now = from.AddHours(12);
            spot.ExpireNoShows(now);

            r.Status.Should().Be(ReservationStatus.Expired);
        }
    }
}