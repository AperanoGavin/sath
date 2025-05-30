using MediatR;

using Microsoft.AspNetCore.Mvc;

using PRS.Application.Commands;
using PRS.Application.Common;
using PRS.Application.Models;
using PRS.Application.Queries;
using PRS.Domain.Enums;
using PRS.Presentation.Models;

namespace PRS.Presentation.Controllers;

[ApiController]
[Route("/api/v1/spots")]
public class SpotController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetSpots(CancellationToken ct)
    {
        var r = await _sender.Send(new GetAllSpotsQuery(), ct);
        if (r.IsFailure) return StatusCode(500, r.Error);
        return Ok(new ApiResponse<IEnumerable<SpotDto>> { Data = [.. r.Value] });
    }

    [HttpGet("capabilities")]
    public async Task<IActionResult> GetCapabilities(CancellationToken ct)
    {
        var capabilities = await _sender.Send(new GetSpotCapabilitiesQuery(), ct).Unwrap();
        return Ok(new ApiResponse<IEnumerable<SpotCapability>> { Data = capabilities });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSpot(Guid id, CancellationToken ct)
    {
        var r = await _sender.Send(new GetSpotByIdQuery(id), ct);
        if (r.IsFailure) return NotFound(r.Error);
        return Ok(new ApiResponse<SpotDto> { Data = r.Value });
    }

    [HttpPost]
    public async Task<IActionResult> CreateSpot(CreateSpotRequest req, CancellationToken ct)
    {
        var r = await _sender.Send(new CreateSpotCommand(req.Key, req.Capabilities), ct);
        if (r.IsFailure) return BadRequest(r.Error);
        return CreatedAtAction(nameof(GetSpot), new { id = r.Value.Id }, new ApiResponse<SpotDto> { Data = r.Value });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> RemoveSpot(Guid id, CancellationToken ct)
    {
        var r = await _sender.Send(new RemoveSpotCommand(id), ct);
        if (r.IsFailure) return NotFound(r.Error);
        return NoContent();
    }

    [HttpGet("{id:guid}/calendar")]
    public async Task<IActionResult> GetCalendar(Guid id, CancellationToken ct)
    {
        var r = await _sender.Send(new GetSpotCalendarQuery(id), ct);
        if (r.IsFailure) return NotFound(r.Error);
        return Ok(new ApiResponse<IEnumerable<ReservationDto>> { Data = [.. r.Value] });
    }
}
