using MediatR;

using PRS.Application.Models;
using PRS.Application.Queries;
using PRS.Domain.Core;
using PRS.Domain.Repositories;

namespace PRS.Application.Handlers;

public class GetAllReservationsHandler(IReservationRepository repo)
        : IRequestHandler<GetAllReservationsQuery, Result<IEnumerable<ReservationDto>>>
{
    private readonly IReservationRepository _repo = repo;

    public async Task<Result<IEnumerable<ReservationDto>>> Handle(
        GetAllReservationsQuery request,
        CancellationToken cancellationToken)
    {
        var all = await _repo.GetAllAsync(cancellationToken);
        var dtos = all.Select(static r => new ReservationDto
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