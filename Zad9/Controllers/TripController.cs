using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zad9.Context;
using Zad9.Exceptions;
using Zad9.Models;
using Zad9.Services;


[Route("api/trips")]
[ApiController]


public class TripController : ControllerBase
{
    private ITripService _tripService;
    public TripController(ITripService tripService)
    {
        _tripService = tripService;
    }
    
    [HttpGet]
    public async Task<IActionResult> GetTrips([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {

        try
        {
            var result = await _tripService.GetTripsAsync(page, pageSize);
            return Ok(result);
        }
        catch (Exception exc)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "server error");
        }
    }

    [HttpDelete("{idClient:int}")]
    public async Task<IActionResult> DeleteClientAsync(int idClient)
    {
        try
        {
            await _tripService.DeleteClientAsync(idClient);
        }
        catch (NoSuchClientException exc )
        {
            return StatusCode(StatusCodes.Status400BadRequest, "no such client");

        }
        catch (ClientHasTripsException exc)
        {
            return StatusCode(StatusCodes.Status400BadRequest, "client has trips saved");

        }

        return Ok();
    }

    [HttpPost("{idTrip:int}/clients")]
    public async Task<IActionResult> AssignClientToTripAsync(AssignClientDto assignClientDto)
    {
        try
        {
            await _tripService.AssignClientToTripAsync(assignClientDto);
            return Ok();
        }
        catch (ClientAlreadyExistsException exc)
        {
            return StatusCode(StatusCodes.Status400BadRequest, "client already exists");
        }
        catch (ClientAlreadyOnThisTripException exc)
        {
            return StatusCode(StatusCodes.Status400BadRequest, "client already on this trip");
        }
        catch (TripAlreadyStartedException exc)
        {
            return StatusCode(StatusCodes.Status400BadRequest, "trip already started");
        }
        catch (NoSuchTripException exc)
        {
            return StatusCode(StatusCodes.Status400BadRequest, "no such trip");
        }
    }

}