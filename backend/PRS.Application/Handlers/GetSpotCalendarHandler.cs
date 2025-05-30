using MediatR;

using PRS.Application.Models;
using PRS.Application.Queries;
using PRS.Domain.Core;
using PRS.Domain.Repositories;

namespace PRS.Application.Handlers;

public class GetSpotCalendarHandler(IReservationRepository repo)
        : IRequestHandler<GetSpotCalendarQuery, Result<IEnumerable<ReservationDto>>>
{
    private readonly IReservationRepository _repo = repo;

    public async Task<Result<IEnumerable<ReservationDto>>> Handle(
        GetSpotCalendarQuery request,
        CancellationToken cancellationToken)
    {
        var res = await _repo.GetBySpotAsync(request.SpotId, cancellationToken);
        var dtos = res.Select(static r => new ReservationDto
        {
            Id = r.Id.ToString(),
            SpotId = r.Spot.Id.ToString(),
            UserId = r.User.Id.ToString(),
            CreatedAt = r.CreatedAt,
            From = r.From,
            To = r.To,
            Status = r.Status
        });
        return Result<IEnumerable<ReservationDto>>.Success(dtos);
    }
}
