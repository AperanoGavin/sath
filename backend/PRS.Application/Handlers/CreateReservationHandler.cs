using MediatR;

using PRS.Application.Behaviors;
using PRS.Application.Commands;
using PRS.Application.Models;
using PRS.Domain.Core;
using PRS.Domain.Errors;
using PRS.Domain.Factories;
using PRS.Domain.Repositories;

namespace PRS.Application.Handlers;

public class CreateReservationHandler(
    IReservationFactory factory,
    IReservationRepository repo,
    IUnitOfWork uow)
        : IRequestHandler<CreateReservationCommand, Result<ReservationDto>>
{
    private readonly IReservationFactory _factory = factory;
    private readonly IReservationRepository _repo = repo;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Result<ReservationDto>> Handle(
        CreateReservationCommand request,
        CancellationToken cancellationToken)
    {
        var r = await _factory.Create(
            request.SpotId, request.UserId, request.From, request.To, cancellationToken);

        if (r.IsFailure)
        {
            throw new DomainErrorException((DomainError)r.Error!);
        }

        var reservation = r.Value;
        await _repo.AddAsync(reservation, cancellationToken);
        await _uow.SaveAsync(cancellationToken);

        return Result<ReservationDto>.Success(new ReservationDto
        {
            Id = reservation.Id.ToString(),
            SpotId = reservation.Spot.Id.ToString(),
            UserId = reservation.User.Id.ToString(),
            CreatedAt = reservation.CreatedAt,
            From = reservation.From,
            To = reservation.To,
            Status = reservation.Status
        });
    }
}
