using MediatR;

using PRS.Application.Models;
using PRS.Domain.Core;

namespace PRS.Application.Commands;

public record CreateReservationCommand(
    Guid SpotId,
    Guid UserId,
    DateTime From,
    DateTime To,
    bool NeedsCharger
) : IRequest<Result<ReservationDto>>;
