using Microsoft.AspNetCore.Mvc;
using PRS.Presentation.Models;

namespace PRS.Presentation.Controllers;

[ApiController]
[Route("/api/v1/reservations")]
public class ReservationController() : ControllerBase
{
    [HttpGet(Name = "GetReservation")]
    public IActionResult GetReservations()
    {
        var mockedReservations = Enumerable.Range(0, 10)
          .Select(static reservation =>
              new ReservationDto
              {
                  Id = Guid.NewGuid().ToString(),
                  SpotId = Guid.NewGuid().ToString(),
                  UserId = Guid.NewGuid().ToString(),
                  CreatedAt = DateTime.UtcNow,
                  From = DateTime.UtcNow,
                  To = DateTime.UtcNow,
                  Status = reservation % 3 == 0 ? ReservationStatus.Booked : ReservationStatus.Reserved
              });

        var response = new ApiResponse<IEnumerable<ReservationDto>>
        {
            Data = mockedReservations,
            Meta = new { Timestamp = DateTime.UtcNow }
        };

        return Ok(response);
    }
}
