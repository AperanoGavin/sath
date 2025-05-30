using MediatR;

using PRS.Domain.Core;

namespace PRS.Application.Commands;

public record CancelReservationCommand(Guid ReservationId) : IRequest<Result>;
