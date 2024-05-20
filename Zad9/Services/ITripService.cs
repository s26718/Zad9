using Zad9.Models;

namespace Zad9.Services;

public interface ITripService
{
    Task<TripsReturnDto> GetTripsAsync(int page, int pageSize);
    Task<bool> DeleteClientAsync(int clientId);
    Task<bool> AssignClientToTripAsync(AssignClientDto assignClientDto);
}