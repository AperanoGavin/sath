using MediatR;

using PRS.Application.Models;
using PRS.Application.Queries;
using PRS.Domain.Core;
using PRS.Domain.Errors;
using PRS.Domain.Repositories;

namespace PRS.Application.Handlers;

public class GetSpotByIdHandler(ISpotRepository repo)
        : IRequestHandler<GetSpotByIdQuery, Result<SpotDto>>
{
    private readonly ISpotRepository _repo = repo;

    public async Task<Result<SpotDto>> Handle(
        GetSpotByIdQuery request,
        CancellationToken cancellationToken)
    {
        var spot = await _repo.GetAsync(request.Id, cancellationToken);
        if (spot is null)
        {
            return Result<SpotDto>.Failure(new SpotNotFoundError(request.Id));
        }

        return Result<SpotDto>.Success(new SpotDto
        {
            Id = spot.Id.ToString(),
            Key = spot.Key,
            Capabilities = [.. spot.Capabilities]
        });
    }
}
