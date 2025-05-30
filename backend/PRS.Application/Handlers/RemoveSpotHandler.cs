using MediatR;

using PRS.Application.Commands;
using PRS.Domain.Core;
using PRS.Domain.Repositories;

namespace PRS.Application.Handlers;

public class RemoveSpotHandler(ISpotRepository repo, IUnitOfWork uow)
        : IRequestHandler<RemoveSpotCommand, Result>
{
    private readonly ISpotRepository _repo = repo;
    private readonly IUnitOfWork _uow = uow;

    public async Task<Result> Handle(RemoveSpotCommand request, CancellationToken cancellationToken)
    {
        await _repo.RemoveAsync(request.Id, cancellationToken);
        await _uow.SaveAsync(cancellationToken);
        return Result.Success();
    }
}