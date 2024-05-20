namespace Zad9.Exceptions;
[Serializable]
public class NoSuchTripException : Exception
{
    public NoSuchTripException ()
    {}

    public NoSuchTripException (string message) 
        : base(message)
    {}

    public NoSuchTripException (string message, Exception innerException)
        : base (message, innerException)
    {}  
}