using MediatR;

using PRS.Domain.Core;

namespace PRS.Application.Commands;

public record CheckInReservationCommand(Guid ReservationId) : IRequest<Result>;
