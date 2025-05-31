using FluentAssertions;

using Moq;

using PRS.Application.Handlers;
using PRS.Application.Queries;
using PRS.Domain.Entities;
using PRS.Domain.Enums;
using PRS.Domain.Errors;
using PRS.Domain.Repositories;

namespace PRS.Application.Tests.Handlers;

public class GetSpotByIdHandlerTests
{
    private readonly Mock<ISpotRepository> _mockRepo = new();
    private readonly GetSpotByIdHandler _handler;

    public GetSpotByIdHandlerTests()
    {
        _handler = new GetSpotByIdHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Handle_WhenSpotDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var missingId = Guid.NewGuid();
        _mockRepo
            .Setup(r => r.GetAsync(missingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Spot?)null);

        // Act
        var result = await _handler.Handle(new GetSpotByIdQuery(missingId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error!.Should().BeOfType<SpotNotFoundError>();
        ((SpotNotFoundError)result.Error).Id.Should().Be(missingId);
    }

    [Fact]
    public async Task Handle_WhenSpotExists_ShouldReturnCorrectDto()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        var existingSpot = new Spot(existingId, "X99");
        existingSpot.AddCapability(SpotCapability.ElectricCharger).IsSuccess.Should().BeTrue();

        _mockRepo
            .Setup(r => r.GetAsync(existingId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSpot);

        // Act
        var result = await _handler.Handle(new GetSpotByIdQuery(existingId), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var dto = result.Value;
        dto.Id.Should().Be(existingId.ToString());
        dto.Key.Should().Be("X99");
        dto.Capabilities.Should().ContainSingle().Which.Should().Be(SpotCapability.ElectricCharger);
    }
}
