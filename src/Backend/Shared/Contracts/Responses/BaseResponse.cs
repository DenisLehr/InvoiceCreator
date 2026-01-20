namespace Shared.Contracts.Responses
{
    public class BaseResponse<T>
    {
        public bool Erfolg { get; set; }
        public string Hinweis { get; set; }
        public T? Daten { get; set; }
        public DateTime Zeitstempel { get; set; }   
    }
}
