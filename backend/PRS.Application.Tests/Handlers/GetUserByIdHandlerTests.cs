using FluentAssertions;

using Moq;

using PRS.Application.Handlers;
using PRS.Application.Queries;
using PRS.Domain.Entities;
using PRS.Domain.Errors;
using PRS.Domain.Repositories;

namespace PRS.Application.Tests.Handlers;

public class GetUserByIdHandlerTests
{
    private readonly Mock<IUserReadOnlyRepository> _mockRepo = new();
    private readonly GetUserByIdHandler _handler;

    public GetUserByIdHandlerTests()
    {
        _handler = new GetUserByIdHandler(_mockRepo.Object);
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotExist_ShouldReturnNotFound()
    {
        // Arrange
        var missingId = Guid.NewGuid();
        _mockRepo.Setup(r => r.GetAsync(missingId, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(new GetUserByIdQuery(missingId), CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<UserNotFoundError>();
        ((UserNotFoundError)result.Error).Id.Should().Be(missingId);
    }

    [Fact]
    public async Task Handle_WhenUserExists_ShouldReturnCorrectDto()
    {
        // Arrange
        var role = new UserRole("Employee", "Employee", "desc");
        var existingUser = new User(Guid.NewGuid(), "Clara", "clara@ex.com", role);

        _mockRepo.Setup(r => r.GetAsync(existingUser.Id, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(existingUser);

        // Act
        var result = await _handler.Handle(new GetUserByIdQuery(existingUser.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var dto = result.Value!;
        dto.Id.Should().Be(existingUser.Id.ToString());
        dto.Name.Should().Be("Clara");
        dto.Email.Should().Be("clara@ex.com");
        dto.Role.Key.Should().Be("Employee");
    }
}