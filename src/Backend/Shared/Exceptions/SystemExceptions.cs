namespace Shared.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException() : base() { }
        public BadRequestException(string message) : base(message) { }
        public BadRequestException(params string[] errors) : base("Mehrere Fehler sind aufgetreten. Siehe Fehlerdetails.")
        {
            Errors = errors;
        }

        public string[] Errors { get; set; }
    }
    
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException() : base("User existiert bereits.") { }
    }

    public class CustomSocketException : Exception
    {
        public CustomSocketException() : base() { }
        
        public CustomSocketException(string message) : base(message) { }

    }

}
