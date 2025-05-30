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

        private Spot() { }

        internal Spot(Guid id,
                     string key,
                     IEnumerable<SpotCapability>? caps = null,
                     IEnumerable<Reservation>? reservations = null)
        {
            Id = id;
            Key = key;
            _caps = caps?.ToList() ?? [];
            _reservations = reservations?.ToList() ?? [];
        }

        public static Result<Spot> Create(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return Result<Spot>.Failure(new SpotInvalidKeyError());
            }

            return Result<Spot>.Success(new Spot(Guid.NewGuid(), key));
        }

        public Result AddCapability(SpotCapability cap)
        {
            if (_caps.Contains(cap))
            {
                return Result.Failure(new SpotDuplicateCapabilityError(cap));
            }

            _caps.Add(cap);
            return Result.Success();
        }

        public Result RemoveCapability(SpotCapability cap)
        {
            if (!_caps.Contains(cap))
            {
                return Result.Failure(new SpotMissingCapabilityError(cap));
            }

            _caps.Remove(cap);
            return Result.Success();
        }

        public async Task<Result<Reservation>> ReserveAsync(User user,
            DateTime from,
            DateTime to,
            IReservationOverlapSpec overlapSpec,
            bool needsCharger = false,
            CancellationToken cancellationToken = default)
        {
            var today = DateTime.UtcNow.Date;
            if (from.Date < today)
                return Result<Reservation>.Failure(new ReservationPastFromError(today));

            if (needsCharger
                && !Capabilities.Contains(SpotCapability.ElectricCharger))
            {
                return Result<Reservation>.Failure(new ReservationCapabilityRequiredError(Key, SpotCapability.ElectricCharger));
            }

            var created = Reservation.Create(this, user, from, to);
            if (created.IsFailure) return created;

            if (!await overlapSpec
                    .IsSatisfiedBy(this, from, to, cancellationToken)
                    .ConfigureAwait(false))
            {
                return Result<Reservation>.Failure(new ReservationOverlapError(Key, from, to));
            }

            _reservations.Add(created.Value);
            return Result<Reservation>.Success(created.Value);
        }

        public Result CheckIn(Guid reservationId, DateTime at)
        {
            var res = _reservations.SingleOrDefault(r => r.Id == reservationId);
            if (res is null)
            {
                return Result.Failure(new ReservationNotFoundError(reservationId));
            }

            return res.CheckIn(at);
        }

        public Result Cancel(Guid reservationId)
        {
            var res = _reservations.SingleOrDefault(r => r.Id == reservationId);
            if (res is null)
            {
                return Result.Failure(new ReservationNotFoundError(reservationId));
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