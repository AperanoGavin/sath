using MediatR;

using PRS.Application.Models;
using PRS.Domain.Core;

namespace PRS.Application.Queries;

public record GetAllReservationsQuery() : IRequest<Result<IEnumerable<ReservationDto>>>;