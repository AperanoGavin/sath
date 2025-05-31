using FluentAssertions;

using Moq;

using PRS.Application.Handlers;
using PRS.Application.Queries;
using PRS.Domain.Entities;
using PRS.Domain.Repositories;

namespace PRS.Application.Tests.Handlers;

public class GetAllUsersHandlerTests
{
    private readonly Mock<IUserReadOnlyRepository> _mockRepo = new();
    private readonly GetAllUsersHandler _handler;

    public GetAllUsersHandlerTests()
    {
        _handler = new GetAllUsersHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Handle_WhenNoUsersExist_ShouldReturnEmpty()
    {
        // Arrange
        _mockRepo.Setup(static r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(new GetAllUsersQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_WhenUsersExist_ShouldMapToDtos()
    {
        // Arrange
        var role = new UserRole("Employee", "Employee", "desc");
        var u1 = new User(Guid.NewGuid(), "Alice", "alice@acme.com", role);
        var u2 = new User(Guid.NewGuid(), "Bob", "bob@acme.com", role);

        _mockRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                 .ReturnsAsync([u1, u2]);

        // Act
        var result = await _handler.Handle(new GetAllUsersQuery(), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var dtos = result.Value!.ToList();
        dtos.Count.Should().Be(2);

        var dto1 = dtos.Single(d => d.Id == u1.Id.ToString());
        dto1.Name.Should().Be("Alice");
        dto1.Email.Should().Be("alice@acme.com");
        dto1.Role.Key.Should().Be("Employee");
    }
}