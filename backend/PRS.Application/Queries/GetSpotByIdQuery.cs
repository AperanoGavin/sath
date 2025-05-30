using MediatR;

using PRS.Application.Models;
using PRS.Domain.Core;

namespace PRS.Application.Queries;

public record GetSpotByIdQuery(Guid Id) : IRequest<Result<SpotDto>>;