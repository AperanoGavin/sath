using MediatR;

using PRS.Application.Behaviors;
using PRS.Application.Commands;
using PRS.Application.Models;
using PRS.Domain.Core;
using PRS.Domain.Errors;
using PRS.Domain.Factories;
using PRS.Domain.Repositories;

namespace PRS.Application.Handlers;

public class CreateSpotHandler(ISpotFactory factory, IUnitOfWork uow)
        : IRequestHandler<CreateSpotCommand, Result<SpotDto>>
{
    private readonly ISpotFactory _factory = factory;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Result<SpotDto>> Handle(
        CreateSpotCommand request,
        CancellationToken cancellationToken)
    {
        var res = await _factory.Create(request.Key, [.. request.Capabilities]);
        if (res.IsFailure)
        {
            throw new DomainErrorException((DomainError)res.Error!);
        }

        var spot = res.Value;
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
