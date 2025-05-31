using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using PRS.Application.Models;
using PRS.Domain.Core;
using PRS.Domain.Errors;
using PRS.Presentation.Controllers;
using PRS.Presentation.Models;
using PRS.Presentation.Tests.Stubs;

namespace PRS.Presentation.Tests.Controllers;

public class UserReadOnlyControllerTests
{
    [Fact]
    public async Task GetUsers_WhenSuccess_ReturnsOkWithData()
    {
        // Arrange
        var dummyUsers = new List<UserDto> {
            new() {
                Id = Guid.NewGuid().ToString(),
                Name = "Alice",
                Email = "alice@ex.com",
                Role = new RoleDto {
                    Id = Guid.NewGuid().ToString(),
                    Key = "Employee",
                    Name = "Employee",
                    Description = "desc"
                }
            }
        };
        var success = Result<IEnumerable<UserDto>>.Success(dummyUsers);
        var stubSender = new StubSender(success);
        var controller = new UserReadOnlyController(stubSender);

        // Act
        var actionResult = await controller.GetUsers(default);

        // Assert
        var ok = actionResult as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.StatusCode.Should().Be(200);

        var wrapper = ok.Value as ApiResponse<IEnumerable<UserDto>>;
        wrapper.Should().NotBeNull();
        wrapper!.Data.Should().BeEquivalentTo(dummyUsers);
    }

    [Fact]
    public async Task GetUsers_WhenFailure_ReturnsProblemDetails()
    {
        var error = new UnknownError("fail");
        var fail = Result<IEnumerable<UserDto>>.Failure(error);
        var stubSender = new StubSender(fail);
        var controller = new UserReadOnlyController(stubSender);

        var actionResult = await controller.GetUsers(default);

        var obj = actionResult as ObjectResult;
        obj.Should().NotBeNull();
        obj!.StatusCode.Should().Be(500);

        var pd = obj.Value as ProblemDetails;
        pd.Should().NotBeNull();
        pd!.Detail.Should().Be(error.Message);
    }

    [Fact]
    public async Task GetUser_WhenNotFound_Returns404()
    {
        var missingId = Guid.NewGuid();
        var error = new UserNotFoundError(missingId);
        var fail = Result<UserDto>.Failure(error);
        var stubSender = new StubSender(fail);
        var controller = new UserReadOnlyController(stubSender);

        var actionResult = await controller.GetUser(missingId, default);

        var obj = actionResult as ObjectResult;
        obj.Should().NotBeNull();
        obj!.StatusCode.Should().Be(404);

        var pd = obj.Value as ProblemDetails;
        pd.Should().NotBeNull();
        pd!.Detail.Should().Be(error.Message);
    }

    [Fact]
    public async Task GetUser_WhenFound_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var dto = new UserDto
        {
            Id = id.ToString(),
            Name = "Bill",
            Email = "bill@ex.com",
            Role = new RoleDto
            {
                Id = Guid.NewGuid().ToString(),
                Key = "Employee",
                Name = "Employee",
                Description = "desc"
            }
        };
        var success = Result<UserDto>.Success(dto);
        var stubSender = new StubSender(success);
        var controller = new UserReadOnlyController(stubSender);

        var actionResult = await controller.GetUser(id, default);

        var ok = actionResult as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.StatusCode.Should().Be(200);

        var wrapper = ok.Value as ApiResponse<UserDto>;
        wrapper.Should().NotBeNull();
        wrapper!.Data.Should().BeEquivalentTo(dto);
    }
}