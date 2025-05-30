using MediatR;

using PRS.Application.Commands;
using PRS.Domain.Core;
using PRS.Domain.Errors;
using PRS.Domain.Repositories;

namespace PRS.Application.Handlers;

public class CancelReservationHandler(
    IReservationRepository repo,
    IUnitOfWork uow)
        : IRequestHandler<CancelReservationCommand, Result>
{
    private readonly IReservationRepository _repo = repo;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Result> Handle(
        CancelReservationCommand request,
        CancellationToken cancellationToken)
    {
        var r = await _repo.GetAsync(request.ReservationId, cancellationToken);
        if (r is null)
        {
            return Result.Failure(new ReservationNotFoundError(request.ReservationId));
        }

        var cancelResult = r.Cancel();
        if (cancelResult.IsFailure)
        {
            return Result.Failure(cancelResult.Error);
        }

        await _repo.UpdateAsync(r, cancellationToken);
        await _uow.SaveAsync(cancellationToken);
        return Result.Success();
    }
}