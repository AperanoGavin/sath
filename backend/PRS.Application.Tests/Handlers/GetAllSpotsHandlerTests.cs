using FluentAssertions;

using Moq;

using PRS.Application.Handlers;
using PRS.Application.Queries;
using PRS.Domain.Entities;
using PRS.Domain.Enums;
using PRS.Domain.Repositories;

namespace PRS.Application.Tests.Handlers;

public class GetAllSpotsHandlerTests
{
    private readonly Mock<ISpotRepository> _mockRepo = new();
    private readonly GetAllSpotsHandler _handler;

    public GetAllSpotsHandlerTests()
    {
        _handler = new GetAllSpotsHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Handle_WhenNoSpotsExist_ShouldReturnEmptyCollection()
    {
        // Arrange
        _mockRepo.Setup(static r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(new GetAllSpotsQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var dtos = result.Value!;
        dtos.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenSpotsExist_ShouldReturnDtos()
    {
        // Arrange
        var spot1 = new Spot(Guid.NewGuid(), "S1");
        spot1.AddCapability(SpotCapability.ElectricCharger).IsSuccess.Should().BeTrue();
        var spot2 = new Spot(Guid.NewGuid(), "S2");
        var spots = new List<Spot> { spot1, spot2 };

        _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync(spots);

        // Act
        var result = await _handler.Handle(new GetAllSpotsQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var dtos = result.Value!.ToList();
        dtos.Count.Should().Be(2);

        // Validate first DTO
        var dto1 = dtos.Single(d => d.Id == spot1.Id.ToString());
        dto1.Key.Should().Be("S1");
        dto1.Capabilities.Should().Contain(SpotCapability.ElectricCharger);

        // Validate second DTO
        var dto2 = dtos.Single(d => d.Id == spot2.Id.ToString());
        dto2.Key.Should().Be("S2");
        dto2.Capabilities.Should().BeEmpty();
    }
}
