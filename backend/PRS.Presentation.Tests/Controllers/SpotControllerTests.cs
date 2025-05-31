using FluentAssertions;

using Microsoft.AspNetCore.Mvc;

using PRS.Application.Models;
using PRS.Domain.Core;
using PRS.Domain.Enums;
using PRS.Domain.Errors;
using PRS.Presentation.Controllers;
using PRS.Presentation.Models;
using PRS.Presentation.Tests.Stubs;

namespace PRS.Presentation.Tests.Controllers;

public class SpotControllerTests
{
    [Fact]
    public async Task GetSpots_WhenSuccess_ReturnsOkWithData()
    {
        // Arrange
        var dummyList = new List<SpotDto> {
            new SpotDto { Id = Guid.NewGuid().ToString(), Key = "Z1", Capabilities = Array.Empty<SpotCapability>() }
        };
        var resultWrapper = Result<IEnumerable<SpotDto>>.Success(dummyList);

        var stubSender = new StubSender(resultWrapper);
        var controller = new SpotController(stubSender);

        // Act
        var actionResult = await controller.GetSpots(default);

        // Assert
        var okResult = actionResult as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var wrapper = okResult.Value as ApiResponse<IEnumerable<SpotDto>>;
        wrapper.Should().NotBeNull();
        wrapper!.Data.Should().BeEquivalentTo(dummyList);
    }

    [Fact]
    public async Task GetSpot_WhenNotFound_Returns404Problem()
    {
        // Arrange
        var error = new SpotNotFoundError(Guid.NewGuid());
        var failureResult = Result<SpotDto>.Failure(error);

        var stubSender = new StubSender(failureResult);
        var controller = new SpotController(stubSender);

        // Act
        var actionResult = await controller.GetSpot(Guid.NewGuid(), default);

        // Assert
        var objectResult = actionResult as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(404);

        var pd = objectResult.Value as ProblemDetails;
        pd.Should().NotBeNull();
        pd!.Detail.Should().Be(error.Message);
        pd.Title.Should().Be(error.Title);
    }

    [Fact]
    public async Task GetSpot_WhenFound_ReturnsOkWithData()
    {
        // Arrange
        var existingId = Guid.NewGuid();
        var dummyDto = new SpotDto
        {
            Id = existingId.ToString(),
            Key = "K1",
            Capabilities = []
        };
        var successResult = Result<SpotDto>.Success(dummyDto);

        var stubSender = new StubSender(successResult);
        var controller = new SpotController(stubSender);

        // Act
        var actionResult = await controller.GetSpot(existingId, CancellationToken.None);

        // Assert
        var okResult = actionResult as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var wrapper = okResult.Value as ApiResponse<SpotDto>;
        wrapper.Should().NotBeNull();

        wrapper!.Data.Should().BeEquivalentTo(dummyDto);
    }


    [Fact]
    public async Task CreateSpot_WhenSuccess_ReturnsCreatedResponse()
    {
        // Arrange
        var newId = Guid.NewGuid();
        var dummyDto = new SpotDto { Id = newId.ToString(), Key = "NEW", Capabilities = new[] { SpotCapability.ElectricCharger } };
        var successResult = Result<SpotDto>.Success(dummyDto);

        var stubSender = new StubSender(successResult);
        var controller = new SpotController(stubSender);

        var createRequest = new CreateSpotRequest
        {
            Key = "NEW",
            Capabilities = [SpotCapability.ElectricCharger]
        };

        // Act
        var actionResult = await controller.CreateSpot(createRequest, default);

        // Assert
        var createdResult = actionResult as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);
        createdResult.ActionName.Should().Be(nameof(SpotController.GetSpot));
        ((string)createdResult.RouteValues!["id"]!).Should().Be(newId.ToString());

        var wrapper = createdResult.Value as ApiResponse<SpotDto>;
        wrapper.Should().NotBeNull();
        wrapper!.Data.Should().BeEquivalentTo(dummyDto);
    }

    [Fact]
    public async Task CreateSpot_WhenFailure_ReturnsProblemDetails()
    {
        // Arrange
        var error = new SpotDuplicateKeyError("DUP");
        var failureResult = Result<SpotDto>.Failure(error);

        var stubSender = new StubSender(failureResult);
        var controller = new SpotController(stubSender);

        var createRequest = new CreateSpotRequest { Key = "DUP", Capabilities = Array.Empty<SpotCapability>() };

        // Act
        var actionResult = await controller.CreateSpot(createRequest, default);

        // Assert
        var objectResult = actionResult as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(409);

        var pd = objectResult.Value as ProblemDetails;
        pd.Should().NotBeNull();
        pd!.Detail.Should().Be(error.Message);
    }

    [Fact]
    public async Task RemoveSpot_WhenSuccess_ReturnsNoContent()
    {
        // Arrange
        var successResult = Result.Success();
        var stubSender = new StubSender(successResult);
        var controller = new SpotController(stubSender);

        var someId = Guid.NewGuid();

        // Act
        var actionResult = await controller.RemoveSpot(someId, default);

        // Assert
        var noContentResult = actionResult as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task RemoveSpot_WhenFailure_ReturnsProblemDetails()
    {
        // Arrange
        var error = new UnknownError("Something bad");
        var failureResult = Result.Failure(error);

        var stubSender = new StubSender(failureResult);
        var controller = new SpotController(stubSender);

        var someId = Guid.NewGuid();

        // Act
        var actionResult = await controller.RemoveSpot(someId, CancellationToken.None);

        // Assert
        var objectResult = actionResult as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);

        var pd = objectResult.Value as ProblemDetails;
        pd.Should().NotBeNull();
        pd!.Detail.Should().Be(error.Message);
    }

    [Fact]
    public async Task GetCalendar_WhenSuccess_ReturnsOkWithData()
    {
        // Arrange
        var spotId = Guid.NewGuid();
        var dummyResList = new List<ReservationDto> {
            new ReservationDto
            {
                Id = Guid.NewGuid().ToString(),
                SpotId = spotId.ToString(),
                UserId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                From = DateTime.UtcNow.Date.AddDays(1),
                To = DateTime.UtcNow.Date.AddDays(2),
                Status = ReservationStatus.Reserved
            }
        };
        var successResult = Result<IEnumerable<ReservationDto>>.Success(dummyResList);

        var stubSender = new StubSender(successResult);
        var controller = new SpotController(stubSender);

        // Act
        var actionResult = await controller.GetCalendar(spotId, default);

        // Assert
        var okResult = actionResult as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);

        var wrapper = okResult.Value as ApiResponse<IEnumerable<ReservationDto>>;
        wrapper.Should().NotBeNull();
        wrapper!.Data.Should().BeEquivalentTo(dummyResList);
    }

    [Fact]
    public async Task GetCalendar_WhenFailure_ReturnsProblemDetails()
    {
        // Arrange
        var error = new SpotNotFoundError(Guid.NewGuid());
        var failureResult = Result<IEnumerable<ReservationDto>>.Failure(error);

        var stubSender = new StubSender(failureResult);
        var controller = new SpotController(stubSender);

        // Act
        var actionResult = await controller.GetCalendar(Guid.NewGuid(), default);

        // Assert
        var objectResult = actionResult as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(404);

        var pd = objectResult.Value as ProblemDetails;
        pd.Should().NotBeNull();
        pd!.Detail.Should().Be(error.Message);
    }
}