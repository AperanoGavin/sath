using MediatR;

using PRS.Application.Models;
using PRS.Application.Queries;
using PRS.Domain.Core;
using PRS.Domain.Repositories;

namespace PRS.Application.Handlers;

public class GetAllUsersHandler(
    IUserReadOnlyRepository repo
  ) : IRequestHandler<GetAllUsersQuery, Result<IEnumerable<UserDto>>>
{
    private readonly IUserReadOnlyRepository _repo = repo;

    public async Task<Result<IEnumerable<UserDto>>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await _repo.GetAllAsync(cancellationToken);
        var dtos = users.Select(static u => new UserDto
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
        });
        return Result<IEnumerable<UserDto>>.Success(dtos);
    }
}
