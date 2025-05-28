using PRS.Domain.Core;
using PRS.Domain.Entities;
using PRS.Domain.Errors;
using PRS.Domain.Repositories;
using PRS.Domain.Specifications;

namespace PRS.Domain.Factories;

public class ReservationFactory(
    ISpotRepository spotRepo,
    IUserReadOnlyRepository userRepo,
    IReservationOverlapSpec overlapSpec) : IReservationFactory
{
    private readonly ISpotRepository _spotRepo = spotRepo;
    private readonly IUserReadOnlyRepository _userRepo = userRepo;
    private readonly IReservationOverlapSpec _overlapSpec = overlapSpec;

    public async Task<Result<Reservation>> Create(
        Guid spotId,
        Guid userId,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken = default)
    {
        var spot = await _spotRepo.GetAsync(spotId, cancellationToken);
        if (spot is null)
        {
            return Result<Reservation>.Failure(new DomainError(
                "Reservation.SpotNotFound",
                "Spot not found",
                $"No parking spot with id '{spotId}'"));
        }

        var user = await _userRepo.GetAsync(userId, cancellationToken);
        if (user is null)
        {
            return Result<Reservation>.Failure(new DomainError(
                "Reservation.UserNotFound",
                "User not found",
                $"No user with id '{userId}'"));
        }

        var creationResult = Reservation.Create(spot, user, from, to);
        if (creationResult.IsFailure)
        {
            return creationResult;
        }

        var reservation = creationResult.Value;

        var isAllowed = await _overlapSpec.IsSatisfiedBy(spot, from, to, cancellationToken);
        if (!isAllowed)
        {
            return Result<Reservation>.Failure(new DomainError(
                "Reservation.Overlap",
                "Time slot unavailable",
                $"Spot '{spot.Key}' is already reserved during {from:yyyy-MM-dd} → {to:yyyy-MM-dd}."));
        }

        return Result<Reservation>.Success(reservation);
    }
}
