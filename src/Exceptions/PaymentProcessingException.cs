namespace ProvaPub.Exceptions;

public class PaymentProcessingException : Exception
{
    public PaymentProcessingException(string message) : base(message) { }
}
