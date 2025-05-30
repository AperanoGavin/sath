using PRS.Domain.Core;
using PRS.Domain.Entities;
using PRS.Domain.Errors;
using PRS.Domain.Repositories;
using PRS.Domain.Specifications;

namespace PRS.Domain.Factories;

public class ReservationFactory(
    ISpotRepository spotRepo,
    IUserReadOnlyRepository userRepo,
    IReservationOverlapSpec overlapSpec
) : IReservationFactory
{
    private readonly ISpotRepository _spotRepo = spotRepo;
    private readonly IUserReadOnlyRepository _userRepo = userRepo;
    private readonly IReservationOverlapSpec _overlapSpec = overlapSpec;

    public async Task<Result<Reservation>> Create(
        Guid spotId,
        Guid userId,
        DateTime from,
        DateTime to,
        bool needsCharger = false,
        CancellationToken cancellationToken = default)
    {
        var spot = await _spotRepo.GetAsync(spotId, cancellationToken);
        if (spot is null)
        {
            return Result<Reservation>.Failure(new ReservationNotFoundError(spotId));
        }

        var user = await _userRepo.GetAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result<Reservation>.Failure(new UserNotFoundError(userId));
        }


        var reserveResult = await spot.ReserveAsync(
            user,
            from,
            to,
            _overlapSpec,
            needsCharger,
            cancellationToken);

        if (reserveResult.IsFailure)
        {
            return reserveResult;
        }

        return reserveResult;
    }
}