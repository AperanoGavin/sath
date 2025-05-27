using Microsoft.AspNetCore.Mvc;
using PRS.Presentation.Models;

namespace PRS.Presentation.Controllers;

[ApiController]
[Route("/api/v1/spots")]
public class SpotController() : ControllerBase
{
    [HttpGet(Name = "GetSpot")]
    public IActionResult GetSpots()
    {
        const int rows = 5;
        const int spotsPerRow = 10;

        var mockedSpots = Enumerable.Range(0, rows)
            .SelectMany(row =>
                Enumerable.Range(1, spotsPerRow).Select(spot => new SpotDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Key = $"{(char)('A' + row)}{spot:D2}",
                    Capabilities = spot % 3 == 0 ? [SpotCapability.ElectricCharger] : []
                })
            );

        var response = new ApiResponse<IEnumerable<SpotDto>>
        {
            Data = mockedSpots,
            Meta = new { Timestamp = DateTime.UtcNow }
        };

        return Ok(response);
    }
}
