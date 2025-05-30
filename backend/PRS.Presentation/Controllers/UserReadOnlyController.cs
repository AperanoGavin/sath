using MediatR;

using Microsoft.AspNetCore.Mvc;

using PRS.Application.Models;
using PRS.Application.Queries;
using PRS.Presentation.Common;
using PRS.Presentation.Models;

namespace PRS.Presentation.Controllers;

[ApiController]
[Route("api/v1/users")]
public class UserReadOnlyController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetUsers(CancellationToken ct)
    {
        var result = await _sender.Send(new GetAllUsersQuery(), ct);
        return result.ToActionResult(
            users => Ok(new ApiResponse<IEnumerable<UserDto>> { Data = users })
        );
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id, CancellationToken ct)
    {
        var result = await _sender.Send(new GetUserByIdQuery(id), ct);
        return result.ToActionResult(
            user => Ok(new ApiResponse<UserDto> { Data = user })
        );
    }
}
