using MediatR;

using Microsoft.AspNetCore.Mvc;

using PRS.Application.Models;
using PRS.Application.Queries;
using PRS.Presentation.Models;

namespace PRS.Presentation.Controllers;

[ApiController]
[Route("/api/v1/spots")]
public class SpotController(ISender sender) : ControllerBase
{
    private readonly ISender _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetSpots(CancellationToken cancellationToken)
    {
        var result = await _sender.Send(new GetAllSpotsQuery(), cancellationToken);
        if (result.IsFailure)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result.Error);
        }

        var response = new ApiResponse<IReadOnlyCollection<SpotDto>>()
        {
            Data = [.. result.Value]
        };
        return Ok(response);
    }

    [HttpGet("capabilities")]
    public IActionResult GetSpotCapabilities()
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public IActionResult GetSpot([FromRoute] string id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public IActionResult CreateSpot([FromBody] CreateSpotRequest request)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public IActionResult RemoveSpot([FromRoute] string id)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}/calendar")]
    public IActionResult GetSpotCalendar([FromRoute] string id)
    {
        throw new NotImplementedException();
    }
}
