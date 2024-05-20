using Microsoft.EntityFrameworkCore;
using Zad9.Context;
using Zad9.Exceptions;
using Zad9.Models;

namespace Zad9.Services;

public class TripService : ITripService
{

    private readonly MasterContext _context;
    public TripService(MasterContext context)
    {
        _context = context;
    }

    public async Task<TripsReturnDto> GetTripsAsync(int page,
        int pageSize)
    {
        var pageIndex = page - 1;
        var tripCount = await _context.Trips.CountAsync();
        var pageCount = System.Math.Ceiling(tripCount / (double)pageSize);
        List<TripDto> trips = await _context.Trips.OrderByDescending(t => t.DateFrom)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .Include(t => t.ClientTrips)
            .ThenInclude(ct => ct.IdClientNavigation)
            .Select(t =>
                new TripDto
                {
                    Name = t.Name,
                    Description = t.Description,
                    DateFrom = t.DateFrom,
                    DateTo = t.DateTo,
                    MaxPeople = t.MaxPeople,
                    Countries = t.IdCountries.Select(country => new CountryDto
                    {
                        Name = country.Name
                    }).ToList(),
                    Clients = t.ClientTrips.Select(
                        clientTrip => new ClientDto
                        {
                            FirstName = clientTrip.IdClientNavigation.FirstName,
                            LastName = clientTrip.IdClientNavigation.LastName
                        }
                    ).ToList()
                }).ToListAsync();


        return new TripsReturnDto
        {
            PageNum = page,
            PageSize = pageSize,
            AllPages = (int) pageCount,
            TripDtos = trips
        };



    }

    public async Task<bool> DeleteClientAsync(int clientId)
    {
        Client? client = await _context.Clients
            .Include(c => c.ClientTrips)
            .FirstOrDefaultAsync(client => client.IdClient == clientId);
        if (client == null)
        {
            throw new NoSuchClientException();
        }
        int tripCount = client.ClientTrips.Count;
        Console.WriteLine(tripCount);
        if (tripCount != 0)
        {
            throw new ClientHasTripsException();
        }

         _context.Clients.Remove(client);
         await _context.SaveChangesAsync();
        return true; 
    }

    public async Task<bool> AssignClientToTripAsync(AssignClientDto assignClientDto)
    {
        DateTime requestDateTime = DateTime.Now;
        //var client = await _context.Clients.FirstOrDefaultAsync(client => client.Pesel == assignClientDto.Pesel);
        var clientExists = 
            await _context.Clients.FirstOrDefaultAsync(client => client.Pesel == assignClientDto.Pesel) != null;
        if (clientExists)
        {
            throw new ClientAlreadyExistsException();
        }

        var clientWithThisPeselOnTrip = await _context.ClientTrips
            .Include(ct => ct.IdClientNavigation)
            .Select(ct => ct.IdClientNavigation)
            .Where(c => c.Pesel == assignClientDto.Pesel)
            .FirstOrDefaultAsync();
        if (clientWithThisPeselOnTrip != null)
        {
            throw new ClientAlreadyOnThisTripException();
        }
            
        

        var trip = await _context.Trips.FirstOrDefaultAsync(t => t.IdTrip == assignClientDto.IdTrip);
        if (trip == null)
        {
            throw new NoSuchTripException();
        }

        if (trip.DateFrom <= DateTime.Now)
        {
            throw new TripAlreadyStartedException();
        }
        //get id 
        var newId = await _context.Clients.MaxAsync(c => c.IdClient) + 1;
        
        //insert client
        await _context.Clients.AddAsync(
            new Client
            {
                IdClient = newId,
                FirstName = assignClientDto.FirstName,
                LastName = assignClientDto.LastName,
                Email = assignClientDto.Email,
                Telephone = assignClientDto.Telephone,
                Pesel = assignClientDto.Pesel
            }
        );

        await _context.ClientTrips.AddAsync(
            new ClientTrip
            {
                IdClient = newId,
                IdTrip = assignClientDto.IdTrip,
                RegisteredAt = requestDateTime,
                PaymentDate = assignClientDto.PaymentDate
            }
        );
        await _context.SaveChangesAsync();
        return true;

    }
    
}