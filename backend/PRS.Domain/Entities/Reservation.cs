using PRS.Domain.Core;
using PRS.Domain.Enums;
using PRS.Domain.Errors;

namespace PRS.Domain.Entities
{
    public class Reservation
    {
        public Guid Id { get; init; }
        public Spot Spot { get; }
        public User User { get; }
        public DateTime CreatedAt { get; }
        public DateTime From { get; private set; }
        public DateTime To { get; private set; }
        public ReservationStatus Status { get; private set; }

        private Reservation(Spot spot, User user, DateTime from, DateTime to)
        {
            Id = Guid.NewGuid();
            Spot = spot;
            User = user;
            CreatedAt = DateTime.UtcNow;
            From = from;
            To = to;
            Status = ReservationStatus.Reserved;
        }

        public static Result<Reservation> Create(
            Spot spot,
            User user,
            DateTime from,
            DateTime to)
        {
            if (to <= from)
            {
                return Result<Reservation>.Failure(new DomainError(
                    "Reservation.InvalidPeriod",
                    "Bad dates",
                    "'To' must come after 'From'."));
            }

            var spanDays = (to - from).TotalDays;
            var max = user.Role.Key == "Manager" ? 30 : 5;
            if (spanDays > max)
            {
                return Result<Reservation>.Failure(new DomainError(
                    "Reservation.TooLong",
                    "Period too long",
                    $"Max reservation length is {max} days."));
            }

            return Result<Reservation>.Success(new Reservation(spot, user, from, to));
        }

        public Result Cancel()
        {

            switch (Status)
            {
                case ReservationStatus.Cancelled or ReservationStatus.Expired:
                    return Result.Failure(new DomainError(
                                    "Reservation.AlreadyCancelled",
                                    "Cannot cancel",
                                    "Reservation is already cancelled or expired."));
                case ReservationStatus.CheckedIn:
                    return Result.Failure(new DomainError(
                                    "Reservation.InvalidState",
                                    "Cannot cancel",
                                    "Reservation is already checked-in."));
            }

            Status = ReservationStatus.Cancelled;
            return Result.Success();
        }

        public Result CheckIn(DateTime at)
        {
            if (Status != ReservationStatus.Reserved)
            {
                return Result.Failure(new DomainError(
                    "Reservation.InvalidState",
                    "Cannot check-in",
                    "Only a ‘Reserved’ booking can be checked-in."));
            }


            if (at > From.Date.AddHours(11))
            {
                return Result.Failure(new DomainError(
                    "Reservation.TooLate",
                    "No-show",
                    "Check-in after 11 AM is forbidden; reservation expired."));
            }

            Status = ReservationStatus.CheckedIn;
            return Result.Success();
        }


        internal Result Expire()
        {
            if (Status != ReservationStatus.Reserved)
            {
                return Result.Failure(new DomainError(
                    "Reservation.InvalidState",
                    "Cannot expire",
                    "Only a ‘Reserved’ booking can be expired."));
            }

            Status = ReservationStatus.Expired;
            return Result.Success();
        }

        public bool Overlaps(DateTime otherFrom, DateTime otherTo)
            => From < otherTo && otherFrom < To;
    }
}