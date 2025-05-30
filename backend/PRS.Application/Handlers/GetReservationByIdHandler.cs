using MediatR;

using PRS.Application.Models;
using PRS.Application.Queries;
using PRS.Domain.Core;
using PRS.Domain.Errors;
using PRS.Domain.Repositories;

namespace PRS.Application.Handlers;

public class GetReservationByIdHandler(IReservationRepository repo)
        : IRequestHandler<GetReservationByIdQuery, Result<ReservationDto>>
{
    private readonly IReservationRepository _repo = repo;

    public async Task<Result<ReservationDto>> Handle(
        GetReservationByIdQuery request,
        CancellationToken cancellationToken)
    {
        var r = await _repo.GetAsync(request.ReservationId, cancellationToken);
        if (r is null)
        {
            return Result<ReservationDto>.Failure(new ReservationNotFoundError(request.ReservationId));
        }

        return Result<ReservationDto>.Success(new ReservationDto
        {
            Id = r.Id.ToString(),
            SpotId = r.Spot.Id.ToString(),
            UserId = r.User.Id.ToString(),
            CreatedAt = r.CreatedAt,
            From = r.From,
            To = r.To,
            Status = r.Status
        });
    }
}