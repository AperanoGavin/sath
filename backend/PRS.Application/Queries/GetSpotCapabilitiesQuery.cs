using MediatR;

using PRS.Domain.Core;
using PRS.Domain.Enums;

namespace PRS.Application.Queries;

public record GetSpotCapabilitiesQuery() : IRequest<Result<IEnumerable<SpotCapability>>>;