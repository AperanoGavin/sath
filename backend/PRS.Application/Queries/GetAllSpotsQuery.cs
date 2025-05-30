using MediatR;

using PRS.Application.Models;
using PRS.Domain.Core;

namespace PRS.Application.Queries;

public record GetAllSpotsQuery() : IRequest<Result<IEnumerable<SpotDto>>>;
