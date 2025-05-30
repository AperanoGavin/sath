using MediatR;

using PRS.Application.Behaviors;
using PRS.Application.Commands;
using PRS.Application.Models;
using PRS.Domain.Core;
using PRS.Domain.Errors;
using PRS.Domain.Factories;
using PRS.Domain.Repositories;

namespace PRS.Application.Handlers;

public class CreateSpotHandler(
    ISpotFactory factory,
    ISpotRepository repo,
    IUnitOfWork uow)
        : IRequestHandler<CreateSpotCommand, Result<SpotDto>>
{
    private readonly ISpotFactory _factory = factory;
    private readonly ISpotRepository _repo = repo;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Result<SpotDto>> Handle(
        CreateSpotCommand request,
        CancellationToken cancellationToken)
    {

        var creation = await _factory.Create(request.Key, [.. request.Capabilities]);
        if (creation.IsFailure)
        {
            throw new DomainErrorException((DomainError)creation.Error!);
        }

        var spot = creation.Value;

        await _repo.AddAsync(spot, cancellationToken);
        await _uow.SaveAsync(cancellationToken);

        var dto = new SpotDto
        {
            Id = spot.Id.ToString(),
            Key = spot.Key,
            Capabilities = [.. spot.Capabilities]
        };

        return Result<SpotDto>.Success(dto);
    }
}

