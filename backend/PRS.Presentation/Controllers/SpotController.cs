using MediatR;

using Microsoft.AspNetCore.Mvc;

using PRS.Application.Commands;
using PRS.Application.Models;
using PRS.Application.Queries;
using PRS.Domain.Enums;
using PRS.Presentation.Common;
using PRS.Presentation.Models;

namespace PRS.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/spots")]
    public class SpotController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;

        [HttpGet]
        public async Task<IActionResult> GetSpots(CancellationToken ct)
        {
            var r = await _sender.Send(new GetAllSpotsQuery(), ct);
            return r.ToActionResult(dtos =>
                Ok(new ApiResponse<IEnumerable<SpotDto>> { Data = dtos }));
        }

        [HttpGet("capabilities")]
        public async Task<IActionResult> GetCapabilities(CancellationToken ct)
        {
            var r = await _sender.Send(new GetSpotCapabilitiesQuery(), ct);
            return r.ToActionResult(caps =>
                Ok(new ApiResponse<IEnumerable<SpotCapability>> { Data = caps }));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetSpot(Guid id, CancellationToken ct)
        {
            var r = await _sender.Send(new GetSpotByIdQuery(id), ct);
            return r.ToActionResult(dto =>
                Ok(new ApiResponse<SpotDto> { Data = dto }));
        }

        [HttpPost]
        public async Task<IActionResult> CreateSpot(
            [FromBody] CreateSpotRequest req,
            CancellationToken ct)
        {
            var cmd = new CreateSpotCommand(req.Key, req.Capabilities);
            var r = await _sender.Send(cmd, ct);
            return r.ToActionResult(dto =>
                CreatedAtAction(nameof(GetSpot),
                                new { id = dto.Id },
                                new ApiResponse<SpotDto> { Data = dto }));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> RemoveSpot(Guid id, CancellationToken ct)
        {
            var r = await _sender.Send(new RemoveSpotCommand(id), ct);
            return r.ToActionResult(NoContent);
        }

        [HttpGet("{id:guid}/calendar")]
        public async Task<IActionResult> GetCalendar(Guid id, CancellationToken ct)
        {
            var r = await _sender.Send(new GetSpotCalendarQuery(id), ct);
            return r.ToActionResult(dtos =>
                Ok(new ApiResponse<IEnumerable<ReservationDto>> { Data = dtos }));
        }
    }
}