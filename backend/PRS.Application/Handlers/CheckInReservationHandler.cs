using MediatR;

using PRS.Application.Commands;
using PRS.Domain.Core;
using PRS.Domain.Errors;
using PRS.Domain.Repositories;

namespace PRS.Application.Handlers;

public class CheckInReservationHandler(
    IReservationRepository repo,
    IUnitOfWork uow)
        : IRequestHandler<CheckInReservationCommand, Result>
{
    private readonly IReservationRepository _repo = repo;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Result> Handle(
        CheckInReservationCommand request,
        CancellationToken cancellationToken)
    {
        var r = await _repo.GetAsync(request.ReservationId, cancellationToken);
        if (r is null)
        {
            return Result.Failure(new ReservationNotFoundError(request.ReservationId));

        }
        var ci = r.CheckIn(DateTime.UtcNow);
        if (ci.IsFailure)
        {
            return Result.Failure(ci.Error);
        }

        await _repo.UpdateAsync(r, cancellationToken);
        await _uow.SaveAsync(cancellationToken);
        return Result.Success();
    }
}