using Microsoft.AspNetCore.Mvc;
using PRS.Presentation.Models;

namespace PRS.Presentation.Controllers;

[ApiController]
[Route("/api/v1/spots")]
public class SpotController : ControllerBase
{
    [HttpGet]
    public IActionResult GetSpots()
    {
        throw new NotImplementedException();
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

