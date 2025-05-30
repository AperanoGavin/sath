using MediatR;

using PRS.Application.Models;
using PRS.Domain.Core;

namespace PRS.Application.Queries;

public record GetReservationByIdQuery(Guid ReservationId) : IRequest<Result<ReservationDto>>;