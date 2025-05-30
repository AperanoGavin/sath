using MediatR;

using PRS.Application.Models;
using PRS.Application.Queries;
using PRS.Domain.Core;
using PRS.Domain.Errors;
using PRS.Domain.Repositories;

namespace PRS.Application.Handlers;

public class GetUserByIdHandler(
    IUserReadOnlyRepository repo
  ) : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserReadOnlyRepository _repo = repo;

    public async Task<Result<UserDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var u = await _repo.GetAsync(request.Id, cancellationToken);
        if (u is null)
        {
            return Result<UserDto>.Failure(new UserNotFoundError(request.Id));
        }

        var dto = new UserDto
        {
            Id = u.Id.ToString(),
            Name = u.Name,
            Email = u.Email,
            Role = new RoleDto
            {
                Id = u.Role.Id.ToString(),
                Key = u.Role.Key,
                Name = u.Role.Name,
                Description = u.Role.Description
            }
        };
        return Result<UserDto>.Success(dto);
    }
}
