namespace API_Project.Exceptions
{
    public class ExternalApiException : Exception
    {
        public ExternalApiException(string message) : base(message) { }
    }

    public class NetworkException : Exception
    {
        public NetworkException(string message, Exception inner) : base(message, inner) { }
    }

    public class DeserializationException : Exception
    {
        public DeserializationException(string message, Exception inner)
            : base(message, inner) { }
    }

    public class UnexpectedException : Exception
    {
        public UnexpectedException(string message, Exception inner) : base(message, inner)
        { }
    }
}
