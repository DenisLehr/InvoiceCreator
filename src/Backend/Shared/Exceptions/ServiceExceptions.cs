

namespace Shared.Exceptions
{
    public class RepositoryException : Exception
    {
        public RepositoryException(): base() { }
        public RepositoryException(string message) : base(message) { }
        public RepositoryException(string message, Exception innerEx) : base(message, innerEx) { }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException() : base() { }
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class ValidationException : Exception
    {
        public ValidationException() : base() { }
        public ValidationException(string message) : base(message) { }
        public ValidationException(string message, Exception innerEx) : base(message, innerEx) { }
        public ValidationException(params string[] errors) : base("Mehrere Validierungsfehler sind aufgetreten. Siehe Fehlerdetails.")
        {
            Errors = errors;
        }
        public string[] Errors { get; set; }
    }
}
