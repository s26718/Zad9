namespace Zad9.Exceptions;
[Serializable]
public class ClientAlreadyOnThisTripException : Exception
{
    public ClientAlreadyOnThisTripException ()
    {}

    public ClientAlreadyOnThisTripException (string message) 
        : base(message)
    {}

    public ClientAlreadyOnThisTripException (string message, Exception innerException)
        : base (message, innerException)
    {}  
}