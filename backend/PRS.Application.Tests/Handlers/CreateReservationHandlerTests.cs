using FluentAssertions;

using Moq;

using PRS.Application.Commands;
using PRS.Application.Handlers;
using PRS.Domain.Core;
using PRS.Domain.Entities;
using PRS.Domain.Errors;
using PRS.Domain.Factories;
using PRS.Domain.Repositories;

namespace PRS.Application.Tests.Handlers
{
    public class CreateReservationHandlerTests
    {
        private readonly Mock<IReservationFactory> _mockFactory = new();
        private readonly Mock<IReservationRepository> _mockRepo = new();
        private readonly Mock<IUnitOfWork> _mockUow = new();
        private readonly CreateReservationHandler _handler;

        public CreateReservationHandlerTests()
        {
            _handler = new CreateReservationHandler(
                _mockFactory.Object,
                _mockRepo.Object,
                _mockUow.Object);
        }

        [Fact]
        public async Task Handle_WhenFactoryReturnsFailure_ShouldPropagateFailure()
        {
            // Arrange
            var fakeError = new ReservationOverlapError("K1", DateTime.Today, DateTime.Today.AddDays(1));
            _mockFactory
                .Setup(static f => f.Create(
                    It.IsAny<Guid>(),
                    It.IsAny<Guid>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<bool>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Reservation>.Failure(fakeError));

            var cmd = new CreateReservationCommand(
                SpotId: Guid.NewGuid(),
                UserId: Guid.NewGuid(),
                From: DateTime.Today,
                To: DateTime.Today.AddDays(1),
                NeedsCharger: false);

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Error!.Code.Should().Be(fakeError.Code);

            // Repo and UoW should not be called
            _mockRepo.Verify(static r => r.AddAsync(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Never);
            _mockUow.Verify(static u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenFactorySucceeds_ShouldAddAndReturnDto()
        {
            // Arrange
            var spot = new Spot(Guid.NewGuid(), "S1");
            var role = new UserRole("Employee", "Employee", "desc");
            var user = new User(Guid.NewGuid(), "Alice", "a@a.com", role);
            var from = DateTime.Today.AddDays(1);
            var to = from.AddDays(1);

            var reservation = new Reservation(spot, user, from, to);

            _mockFactory
                .Setup(f => f.Create(
                    spot.Id,
                    user.Id,
                    from,
                    to,
                    false,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result<Reservation>.Success(reservation));

            var cmd = new CreateReservationCommand(
                SpotId: spot.Id,
                UserId: user.Id,
                From: from,
                To: to,
                NeedsCharger: false);

            // Act
            var result = await _handler.Handle(cmd, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            var dto = result.Value;
            dto.Id.Should().Be(reservation.Id.ToString());
            dto.SpotId.Should().Be(spot.Id.ToString());
            dto.UserId.Should().Be(user.Id.ToString());
            dto.From.Should().Be(from);
            dto.To.Should().Be(to);
            dto.Status.Should().Be(reservation.Status);

            // Verify calls
            _mockRepo.Verify(r => r.AddAsync(reservation, It.IsAny<CancellationToken>()), Times.Once);
            _mockUow.Verify(u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}