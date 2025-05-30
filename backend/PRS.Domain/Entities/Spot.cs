using PRS.Domain.Core;
using PRS.Domain.Enums;
using PRS.Domain.Errors;
using PRS.Domain.Specifications;

namespace PRS.Domain.Entities
{
    public class Spot
    {
        public Guid Id { get; init; }
        public string Key { get; init; }
        public IReadOnlyCollection<SpotCapability> Capabilities => _caps.AsReadOnly();

        private readonly List<SpotCapability> _caps;
        private readonly List<Reservation> _reservations;

        public IReadOnlyCollection<Reservation> Reservations => _reservations.AsReadOnly();

        private Spot(Guid id,
                     string key,
                     IEnumerable<SpotCapability>? caps = null,
                     IEnumerable<Reservation>? reservations = null)
        {
            Id = id;
            Key = key;
            _caps = caps?.ToList() ?? new List<SpotCapability>();
            _reservations = reservations?.ToList() ?? new List<Reservation>();
        }

        public static Result<Spot> Create(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return Result<Spot>.Failure(new DomainError(
                    "Spot.InvalidKey",
                    "Empty key",
                    "Spot key cannot be empty."));
            }

            return Result<Spot>.Success(new Spot(Guid.NewGuid(), key));
        }

        public Result AddCapability(SpotCapability cap)
        {
            if (_caps.Contains(cap))
            {
                return Result.Failure(new DomainError(
                    "Spot.DuplicateCapability",
                    "Capability already exists",
                    $"Capability '{cap}' is already assigned."));
            }

            _caps.Add(cap);
            return Result.Success();
        }

        public Result RemoveCapability(SpotCapability cap)
        {
            if (!_caps.Contains(cap))
            {
                return Result.Failure(new DomainError(
                    "Spot.DoesNotHaveCapability",
                    "Capability not assigned",
                    $"Capability '{cap}' is not assigned."));
            }

            _caps.Remove(cap);
            return Result.Success();
        }

        public async Task<Result<Reservation>> ReserveAsync(
            User user,
            DateTime from,
            DateTime to,
            IReservationOverlapSpec overlapSpec,
            CancellationToken cancellationToken = default)
        {

            var todayUtc = DateTime.UtcNow.Date;
            if (from.Date < todayUtc)
            {
                return Result<Reservation>.Failure(new DomainError(
                    "Reservation.PastFrom",
                    "Start date in the past",
                    $"Cannot start before {todayUtc:yyyy-MM-dd}."));
            }


            var creation = Reservation.Create(this, user, from, to);
            if (creation.IsFailure)
                return creation;


            var allowed = await overlapSpec
                .IsSatisfiedBy(this, from, to, cancellationToken)
                .ConfigureAwait(false);
            if (!allowed)
            {
                return Result<Reservation>.Failure(new DomainError(
                    "Reservation.Overlap",
                    "Time slot unavailable",
                    $"Spot '{Key}' is already reserved between {from:yyyy-MM-dd} and {to:yyyy-MM-dd}."));
            }


            var reservation = creation.Value;
            _reservations.Add(reservation);

            return Result<Reservation>.Success(reservation);
        }

        public Result CheckIn(Guid reservationId, DateTime at)
        {
            var res = _reservations.SingleOrDefault(r => r.Id == reservationId);
            if (res is null)
                return Result.Failure(new DomainError(
                    "Reservation.NotFound",
                    "Not found",
                    $"No reservation with ID '{reservationId}'."));

            return res.CheckIn(at);
        }

        public Result Cancel(Guid reservationId)
        {
            var res = _reservations.SingleOrDefault(r => r.Id == reservationId);
            if (res is null)
            {
                return Result.Failure(new DomainError(
                            "Reservation.NotFound",
                            "Not found",
                            $"No reservation with ID '{reservationId}'."));
            }

            return res.Cancel();
        }

        public void ExpireNoShows(DateTime now)
        {
            foreach (var r in _reservations
                         .Where(r => r.Status == ReservationStatus.Reserved
                                  && now > r.From.Date.AddHours(11)))
            {
                r.Expire();
            }
        }
    }
}