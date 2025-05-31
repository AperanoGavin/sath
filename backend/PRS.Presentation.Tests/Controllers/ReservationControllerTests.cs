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

public class ReservationControllerTests
{
    [Fact]
    public async Task GetAll_WhenSuccess_ReturnsOkWithData()
    {
        var dummyList = new List<ReservationDto>
        {
            new() {
                Id = Guid.NewGuid().ToString(),
                SpotId = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                From = DateTime.UtcNow.Date.AddDays(1),
                To = DateTime.UtcNow.Date.AddDays(2),
                Status = ReservationStatus.Reserved
            }
        };
        var success = Result<IEnumerable<ReservationDto>>.Success(dummyList);
        var stubSender = new StubSender(success);
        var controller = new ReservationController(stubSender);

        var actionResult = await controller.GetAll(default);

        var ok = actionResult as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.StatusCode.Should().Be(200);

        var wrapper = ok.Value as ApiResponse<IEnumerable<ReservationDto>>;
        wrapper.Should().NotBeNull();
        wrapper!.Data.Should().BeEquivalentTo(dummyList);
    }

    [Fact]
    public async Task GetById_WhenNotFound_Returns404()
    {
        var missingId = Guid.NewGuid();
        var error = new ReservationNotFoundError(missingId);
        var fail = Result<ReservationDto>.Failure(error);
        var stubSender = new StubSender(fail);
        var controller = new ReservationController(stubSender);

        var actionResult = await controller.GetById(missingId, default);

        var obj = actionResult as ObjectResult;
        obj.Should().NotBeNull();
        obj!.StatusCode.Should().Be(404);

        var pd = obj.Value as ProblemDetails;
        pd.Should().NotBeNull();
        pd!.Detail.Should().Be(error.Message);
    }

    [Fact]
    public async Task GetById_WhenFound_ReturnsOk()
    {
        var id = Guid.NewGuid();
        var dto = new ReservationDto
        {
            Id = id.ToString(),
            SpotId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow,
            From = DateTime.UtcNow.Date.AddDays(1),
            To = DateTime.UtcNow.Date.AddDays(2),
            Status = ReservationStatus.Reserved
        };
        var success = Result<ReservationDto>.Success(dto);
        var stubSender = new StubSender(success);
        var controller = new ReservationController(stubSender);

        var actionResult = await controller.GetById(id, default);

        var ok = actionResult as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.StatusCode.Should().Be(200);

        var wrapper = ok.Value as ApiResponse<ReservationDto>;
        wrapper.Should().NotBeNull();
        wrapper!.Data.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task Create_WhenSuccess_ReturnsCreated()
    {
        var newId = Guid.NewGuid();
        var dto = new ReservationDto
        {
            Id = newId.ToString(),
            SpotId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow,
            From = DateTime.UtcNow.Date.AddDays(1),
            To = DateTime.UtcNow.Date.AddDays(2),
            Status = ReservationStatus.Reserved
        };
        var success = Result<ReservationDto>.Success(dto);
        var stubSender = new StubSender(success);
        var controller = new ReservationController(stubSender);

        var req = new CreateReservationRequest
        {
            SpotId = Guid.Parse(dto.SpotId),
            UserId = Guid.Parse(dto.UserId),
            From = dto.From,
            To = dto.To,
            NeedsCharger = false
        };

        var actionResult = await controller.Create(req, default);

        var created = actionResult as CreatedAtActionResult;
        created.Should().NotBeNull();
        created!.StatusCode.Should().Be(201);
        ((string)created.RouteValues!["id"]!).Should().Be(newId.ToString());

        var wrapper = created.Value as ApiResponse<ReservationDto>;
        wrapper.Should().NotBeNull();
        wrapper!.Data.Should().BeEquivalentTo(dto);
    }

    [Fact]
    public async Task Create_WhenFailure_ReturnsProblemDetails()
    {
        var error = new ReservationOverlapError("KEYX", DateTime.Today, DateTime.Today.AddDays(1));
        var fail = Result<ReservationDto>.Failure(error);
        var stubSender = new StubSender(fail);
        var controller = new ReservationController(stubSender);

        var req = new CreateReservationRequest
        {
            SpotId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            From = DateTime.Today,
            To = DateTime.Today.AddDays(1),
            NeedsCharger = false
        };

        var actionResult = await controller.Create(req, default);

        var obj = actionResult as ObjectResult;
        obj.Should().NotBeNull();
        obj!.StatusCode.Should().Be(409);

        var pd = obj.Value as ProblemDetails;
        pd.Should().NotBeNull();
        pd!.Detail.Should().Be(error.Message);
    }

    [Fact]
    public async Task Cancel_WhenSuccess_ReturnsNoContent()
    {
        var success = Result.Success();
        var stubSender = new StubSender(success);
        var controller = new ReservationController(stubSender);
        var someId = Guid.NewGuid();

        var actionResult = await controller.Cancel(someId, default);

        var noContent = actionResult as NoContentResult;
        noContent.Should().NotBeNull();
        noContent!.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task Cancel_WhenFailure_ReturnsProblemDetails()
    {
        var missingId = Guid.NewGuid();
        var error = new ReservationNotFoundError(missingId);
        var fail = Result.Failure(error);
        var stubSender = new StubSender(fail);
        var controller = new ReservationController(stubSender);

        var actionResult = await controller.Cancel(missingId, default);

        var obj = actionResult as ObjectResult;
        obj.Should().NotBeNull();
        obj!.StatusCode.Should().Be(404);

        var pd = obj.Value as ProblemDetails;
        pd.Should().NotBeNull();
        pd!.Detail.Should().Be(error.Message);
    }

    [Fact]
    public async Task CheckIn_WhenSuccess_ReturnsNoContent()
    {
        var success = Result.Success();
        var stubSender = new StubSender(success);
        var controller = new ReservationController(stubSender);
        var id = Guid.NewGuid();

        var actionResult = await controller.CheckIn(id, default);

        var noContent = actionResult as NoContentResult;
        noContent.Should().NotBeNull();
        noContent!.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task CheckIn_WhenFailure_ReturnsProblemDetails()
    {
        var missingId = Guid.NewGuid();
        var error = new ReservationNotFoundError(missingId);
        var fail = Result.Failure(error);
        var stubSender = new StubSender(fail);
        var controller = new ReservationController(stubSender);

        var actionResult = await controller.CheckIn(missingId, default);

        var obj = actionResult as ObjectResult;
        obj.Should().NotBeNull();
        obj!.StatusCode.Should().Be(404);

        var pd = obj.Value as ProblemDetails;
        pd.Should().NotBeNull();
        pd!.Detail.Should().Be(error.Message);
    }

    [Fact]
    public async Task History_WhenSuccess_ReturnsOkWithData()
    {
        var dummyList = new List<ReservationDto>
        {
            new()
            {
                Id = Guid.NewGuid().ToString(),
                SpotId = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                From = DateTime.UtcNow.Date.AddDays(-3),
                To = DateTime.UtcNow.Date.AddDays(-1),
                Status = ReservationStatus.Expired
            }
        };
        var success = Result<IEnumerable<ReservationDto>>.Success(dummyList);
        var stubSender = new StubSender(success);
        var controller = new ReservationController(stubSender);

        var actionResult = await controller.History(default);

        var ok = actionResult as OkObjectResult;
        ok.Should().NotBeNull();
        ok!.StatusCode.Should().Be(200);

        var wrapper = ok.Value as ApiResponse<IEnumerable<ReservationDto>>;
        wrapper.Should().NotBeNull();
        wrapper!.Data.Should().BeEquivalentTo(dummyList);
    }
}