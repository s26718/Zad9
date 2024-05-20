namespace Zad9.Exceptions;
[Serializable]
public class NoSuchClientException : Exception
{
    public NoSuchClientException ()
    {}

    public NoSuchClientException (string message) 
        : base(message)
    {}

    public NoSuchClientException (string message, Exception innerException)
        : base (message, innerException)
    {}  
}