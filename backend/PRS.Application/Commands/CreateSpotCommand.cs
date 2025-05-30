using MediatR;

using PRS.Application.Models;
using PRS.Domain.Core;
using PRS.Domain.Enums;

namespace PRS.Application.Commands;

public record CreateSpotCommand(
    string Key,
    IEnumerable<SpotCapability> Capabilities
) : IRequest<Result<SpotDto>>;
