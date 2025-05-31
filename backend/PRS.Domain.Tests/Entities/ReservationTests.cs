using FluentAssertions;

using PRS.Domain.Entities;

namespace PRS.Domain.Tests.Entities;

public class ReservationTests
{
    private readonly Spot _spot = Spot.Create("Z10").Value;
    private readonly UserRole _employeeRole = new("Employee", "Employee", "This is the employee role.");
    private readonly UserRole _managerRole = new("Manager", "Manager", "This is the dictator role.");
    private readonly User _user;

    public ReservationTests()
    {
        _user = User.Create("Alice", "alice@acme.com", _employeeRole).Value;
    }

    [Fact]
    public void Create_Allows_5_Working_Days_Across_Weekend()
    {
        var from = new DateTime(2025, 06, 02);
        var to = new DateTime(2025, 06, 09);

        var result = Reservation.Create(_spot, _user, from, to);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void Create_Rejects_6_Working_Days_Across_Weekend()
    {
        var from = new DateTime(2025, 06, 02);
        var to = new DateTime(2025, 06, 10);

        var result = Reservation.Create(_spot, _user, from, to);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be("Reservation.TooLong");
    }


    [Fact]
    public void Create_InvalidPeriod_ReturnsFailure()
    {
        // Arrange
        var from = DateTime.Today.AddDays(1);
        var to = from.AddDays(-1);

        // Act
        var r = Reservation.Create(_spot, _user, from, to);

        // Assert
        r.IsFailure.Should().BeTrue();
        r.Error!.Code.Should().Be("Reservation.InvalidPeriod");
    }

    [Fact]
    public void Create_ExceedsMaxDaysForEmployee_ReturnsFailure()
    {
        // Arrange
        var from = new DateTime(2025, 6, 2);
        var to = new DateTime(2025, 6, 10);

        // Act
        var r = Reservation.Create(_spot, _user, from, to);

        // Assert
        r.IsFailure.Should().BeTrue();
        r.Error!.Code.Should().Be("Reservation.TooLong");
    }

    [Fact]
    public void Create_ManagerCanBook30Days()
    {
        // Arrange
        var mgr = User.Create("Bob", "bob@acme.com", _managerRole).Value;
        var from = DateTime.Today;
        var to = from.AddDays(30);

        // Act
        var r = Reservation.Create(_spot, mgr, from, to);

        // Assert
        r.IsSuccess.Should().BeTrue();
        r.Value.To.Should().Be(to);
    }

    [Fact]
    public void CheckIn_NotReservedSpot_ReturnsInvalidStateFailure()
    {
        // Arrange
        var from = DateTime.Today;
        var to = from.AddDays(3);
        var noon = from.Date.AddHours(10);

        // Act
        var res = Reservation.Create(_spot, _user, from, to).Value;
        res.Cancel();
        var check = res.CheckIn(noon);

        // Assert
        check.IsFailure.Should().BeTrue();
        check.Error!.Code.Should().Be("Reservation.InvalidState");
    }

    [Fact]
    public void CheckIn_After11AM_ReturnsTooLateFailure()
    {
        // Arrange
        var from = DateTime.Today;
        var to = from.AddDays(1);
        var noon = from.Date.AddHours(12);

        // Act
        var res = Reservation.Create(_spot, _user, from, to).Value;
        var check = res.CheckIn(noon);

        // Assert
        check.IsFailure.Should().BeTrue();
        check.Error!.Code.Should().Be("Reservation.TooLate");
    }

    [Fact]
    public void Cancel_Twice_ReturnsAlreadyCancelledFailureOnSecond()
    {
        // Arrange
        var from = DateTime.Today;
        var to = from.AddDays(1);

        // Act & Assert
        var res = Reservation.Create(_spot, _user, from, to).Value;

        res.Cancel().IsSuccess.Should().BeTrue();
        var second = res.Cancel();
        second.IsFailure.Should().BeTrue();
        second.Error!.Code.Should().Be("Reservation.AlreadyCancelled");
    }
}