using FluentAssertions;

using Moq;

using PRS.Application.Commands;
using PRS.Application.Handlers;
using PRS.Domain.Core;
using PRS.Domain.Entities;
using PRS.Domain.Enums;
using PRS.Domain.Errors;
using PRS.Domain.Factories;
using PRS.Domain.Repositories;

namespace PRS.Application.Tests.Handlers;

public class CreateSpotHandlerTests
{
    private readonly Mock<ISpotFactory> _mockFactory = new();
    private readonly Mock<ISpotRepository> _mockRepo = new();
    private readonly Mock<IUnitOfWork> _mockUow = new();
    private readonly CreateSpotHandler _handler;

    public CreateSpotHandlerTests()
    {
        _handler = new CreateSpotHandler(
            _mockFactory.Object,
            _mockRepo.Object,
            _mockUow.Object);
    }

    [Fact]
    public async Task Handle_WhenFactoryReturnsFailure_ShouldReturnFailureWithSameError()
    {
        // Arrange
        var error = new SpotDuplicateKeyError("DUPLICATE_KEY");
        _mockFactory
            .Setup(static f => f.Create(It.IsAny<string>(), It.IsAny<ICollection<SpotCapability>>()))
            .ReturnsAsync(Result<Spot>.Failure(error));

        var command = new CreateSpotCommand("DUPLICATE_KEY", Array.Empty<SpotCapability>());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be(error.Code);

        // It should never attempt to add to repository or save
        _mockRepo.Verify(static r => r.AddAsync(It.IsAny<Spot>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockUow.Verify(static u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenFactorySucceeds_ShouldAddSpotAndReturnDto()
    {
        // Arrange
        var fakeId = Guid.NewGuid();
        var fakeSpot = new Spot(fakeId, "KEY123");
        fakeSpot.AddCapability(SpotCapability.ElectricCharger).IsSuccess.Should().BeTrue();

        _mockFactory
            .Setup(f => f.Create(
                "KEY123",
                It.Is<ICollection<SpotCapability>>(caps => caps.Contains(SpotCapability.ElectricCharger))
            ))
            .ReturnsAsync(Result<Spot>.Success(fakeSpot));

        var command = new CreateSpotCommand("KEY123", [SpotCapability.ElectricCharger]);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var dto = result.Value;
        dto.Id.Should().Be(fakeId.ToString());
        dto.Key.Should().Be("KEY123");
        dto.Capabilities.Should().ContainSingle().Which.Should().Be(SpotCapability.ElectricCharger);

        _mockRepo.Verify(r => r.AddAsync(fakeSpot, It.IsAny<CancellationToken>()), Times.Once);
        _mockUow.Verify(u => u.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

}