namespace Zad9.Models;

public class TripsReturnDto
{
    public int PageNum { get; set; } 
    public int PageSize { get; set; }
    public int AllPages { get; set; }
    public List<TripDto> TripDtos { get; set; }
}