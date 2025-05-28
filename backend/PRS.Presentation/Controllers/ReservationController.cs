using Microsoft.AspNetCore.Mvc;
using PRS.Presentation.Models;

namespace PRS.Presentation.Controllers;

[ApiController]
[Route("/api/v1/reservations")]
public class ReservationController() : ControllerBase
{
    [HttpGet]
    public IActionResult GetReservations()
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public IActionResult CreateReservation([FromBody] CreateReservationRequest request)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public IActionResult GetReservation([FromRoute] string id)
    {
        throw new NotImplementedException();
    }

    [HttpPut("{id}/cancel")]
    public IActionResult CancelReservation([FromRoute] string id)
    {
        throw new NotImplementedException();
    }

    [HttpPost("{id}/check-in")]
    public IActionResult CheckInReservation([FromRoute] string id)
    {
        throw new NotImplementedException();
    }

    [HttpGet("history")]
    public IActionResult GetReservationHistory()
    {
        throw new NotImplementedException();
    }
}

