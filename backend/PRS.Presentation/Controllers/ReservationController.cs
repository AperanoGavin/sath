using MediatR;

using Microsoft.AspNetCore.Mvc;

using PRS.Application.Commands;
using PRS.Application.Models;
using PRS.Application.Queries;
using PRS.Presentation.Common;
using PRS.Presentation.Models;

namespace PRS.Presentation.Controllers
{
    [ApiController]
    [Route("api/v1/reservations")]
    public class ReservationController(ISender sender) : ControllerBase
    {
        private readonly ISender _sender = sender;

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var r = await _sender.Send(new GetAllReservationsQuery(), ct);
            return r.ToActionResult(dtos =>
                Ok(new ApiResponse<IEnumerable<ReservationDto>> { Data = dtos }));
        }

        [HttpGet("history")]
        public async Task<IActionResult> History(CancellationToken ct)
        {
            var r = await _sender.Send(new GetReservationHistoryQuery(), ct);
            return r.ToActionResult(dtos =>
                Ok(new ApiResponse<IEnumerable<ReservationDto>> { Data = dtos }));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var r = await _sender.Send(new GetReservationByIdQuery(id), ct);
            return r.ToActionResult(dto =>
                Ok(new ApiResponse<ReservationDto> { Data = dto }));
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateReservationRequest req,
            CancellationToken ct)
        {
            var cmd = new CreateReservationCommand(
                req.SpotId,
                req.UserId,
                req.From,
                req.To,
                req.NeedsCharger);
            var r = await _sender.Send(cmd, ct);
            return r.ToActionResult(dto =>
                CreatedAtAction(nameof(GetById),
                                new { id = dto.Id },
                                new ApiResponse<ReservationDto> { Data = dto }));
        }

        [HttpPut("{id:guid}/cancel")]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
        {
            var r = await _sender.Send(new CancelReservationCommand(id), ct);
            return r.ToActionResult(NoContent);
        }

        [HttpPost("{id:guid}/check-in")]
        public async Task<IActionResult> CheckIn(Guid id, CancellationToken ct)
        {
            var r = await _sender.Send(new CheckInReservationCommand(id), ct);
            return r.ToActionResult(NoContent);
        }
    }
}