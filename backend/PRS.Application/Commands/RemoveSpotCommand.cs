using MediatR;

using PRS.Domain.Core;

namespace PRS.Application.Commands;

public record RemoveSpotCommand(Guid Id) : IRequest<Result>;
