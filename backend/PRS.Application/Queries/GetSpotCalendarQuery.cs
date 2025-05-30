using MediatR;

using PRS.Application.Models;
using PRS.Domain.Core;

namespace PRS.Application.Queries;

public record GetSpotCalendarQuery(Guid SpotId) : IRequest<Result<IEnumerable<ReservationDto>>>;