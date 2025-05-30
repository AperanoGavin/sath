using MediatR;

using PRS.Application.Models;
using PRS.Application.Queries;
using PRS.Domain.Core;
using PRS.Domain.Repositories;

namespace PRS.Application.Handlers;

public class GetAllSpotsHandler(ISpotRepository repo)
        : IRequestHandler<GetAllSpotsQuery, Result<IEnumerable<SpotDto>>>
{
    private readonly ISpotRepository _repo = repo;

    public async Task<Result<IEnumerable<SpotDto>>> Handle(
        GetAllSpotsQuery request,
        CancellationToken cancellationToken)
    {
        var spots = await _repo.GetAllAsync(cancellationToken);
        var dtos = spots.Select(static s => new SpotDto
        {
            Id = s.Id.ToString(),
            Key = s.Key,
            Capabilities = [.. s.Capabilities]
        });
        return Result<IEnumerable<SpotDto>>.Success(dtos);
    }
}
