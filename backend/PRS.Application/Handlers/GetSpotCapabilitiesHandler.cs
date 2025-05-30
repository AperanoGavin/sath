using MediatR;

using PRS.Application.Queries;
using PRS.Domain.Core;
using PRS.Domain.Enums;

namespace PRS.Application.Handlers;

public class GetSpotCapabilitiesHandler
    : IRequestHandler<GetSpotCapabilitiesQuery, Result<IEnumerable<SpotCapability>>>
{
    public Task<Result<IEnumerable<SpotCapability>>> Handle(
        GetSpotCapabilitiesQuery request,
        CancellationToken cancellationToken)
    {
        var all = Enum.GetValues<SpotCapability>();
        return Task.FromResult(
            Result<IEnumerable<SpotCapability>>.Success(all));
    }
}
